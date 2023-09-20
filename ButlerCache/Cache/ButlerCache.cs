using BryanButler.Cache.Exceptions;
using BryanButler.Cache.Models;
using BryanButler.Cache.Services;

namespace BryanButler.Cache;
public class ButlerCache : CacheService
{
    private static readonly Lazy<ButlerCache> SingletonInstance = new(() => new ButlerCache());
    public static ButlerCache Instance => SingletonInstance.Value;
    private ButlerCache() { }
    
    private int _capacity = 100;
    private readonly object _lock = new();
    public bool Add(string key, object value)
    {
        lock (_lock)
        {
            if (CacheDictionary.Count >= _capacity)
                RemoveLeastRecentlyUsedItem();

            return CacheDictionary.TryAdd(key, CreateNewCacheItem(key, value));
        }
    }

    public T Get<T>(string key)
    {
        lock (_lock)
        {
            var cacheItem = GetCacheItem(key);
            if (cacheItem == null)
                throw new CacheItemKeyNotFoundException(key);

            var typeOfT = typeof(T).ToString();
            if (cacheItem.CacheType != typeOfT)
                throw new CacheItemTypeIsIncorrectException(key, cacheItem.CacheType, typeOfT);

            return (T)cacheItem.CacheValue;
        }
    }

    public bool Remove(string key) => RemoveByKey(key);

    public int RemainingCapacity() => _capacity - CacheDictionary.Count;

    public void SetCapacity(int newCapacity)
    {
        _capacity = newCapacity;

        while (RemainingCapacity() < 0)
            RemoveLeastRecentlyUsedItem();
    }

    public void Clear() => CacheDictionary.Clear();
}