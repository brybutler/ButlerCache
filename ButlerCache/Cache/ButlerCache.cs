using BryanButler.Cache.Exceptions;
using BryanButler.Cache.Services;
namespace BryanButler.Cache;
public class ButlerCache : CacheService
{
    private static readonly Lazy<ButlerCache> SingletonInstance = new(() => new ButlerCache());
    public static ButlerCache Instance => SingletonInstance.Value;
    private ButlerCache() { }
    
    private readonly object _lock = new();
    private int _capacity = 100;
    private readonly int _timeout = 100;
    public bool Add(string key, object value)
    {
        bool lockTaken = false;
        try
        {
            Monitor.TryEnter(_lock, _timeout, ref lockTaken);
            if (lockTaken)
            {
                if (Cache.Count >= _capacity)
                    RemoveLeastRecentlyUsedItem();

                var newCacheItem = CreateNewCacheItem(key, value);
                if (Cache.TryAdd(key, newCacheItem))
                {
                    if (IsKeyInEvictedList(key))
                        EvictedKeys.Remove(key);

                    LruCacheList.AddFirst(newCacheItem);
                    return true;
                }
            }
        }
        finally
        {
            if (lockTaken)
                Monitor.Exit(_lock);
        }

        return false;
    }

    public T Get<T>(string key)
    {
        if (IsKeyInEvictedList(key))
            throw new CacheItemEvictedException(key);

        var cacheItem = GetCacheItem(key);
        if (cacheItem == null)
            throw new CacheItemKeyNotFoundException(key);

        var typeOfT = typeof(T).ToString();
        if (cacheItem.Value.CacheType != typeOfT)
            throw new CacheItemTypeIsIncorrectException(key, cacheItem.Value.CacheType, typeOfT);

        LruCacheList.Remove(cacheItem);
        LruCacheList.AddFirst(cacheItem);

        return (T)cacheItem.Value.CacheValue;
    }

    public bool Remove(string key) => RemoveCacheItem(GetCacheItem(key));
    public LinkedList<string> GetEvictedKeys() => EvictedKeys;
    public int RemainingCapacity() => _capacity - LruCacheList.Count;

    public void SetCapacity(int newCapacity)
    {
        _capacity = newCapacity;

        while (RemainingCapacity() < 0)
            RemoveLeastRecentlyUsedItem();
    }

    public void Clear()
    {
        Cache.Clear();
        LruCacheList.Clear();
        EvictedKeys.Clear();
    }
}