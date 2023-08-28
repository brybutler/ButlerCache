using BryanButler.Cache.Exceptions;
using BryanButler.Cache.Models;
using System.Collections.Concurrent;
namespace BryanButler.Cache.Services;
public class CacheService
{
    protected LinkedList<string> EvictedKeys = new();
    protected LinkedList<CacheItem> LruCacheList = new();
    protected ConcurrentDictionary<string, LinkedListNode<CacheItem>> Cache = new();
    
    protected bool IsKeyInEvictedList(string key) => EvictedKeys.Contains(key);

    protected LinkedListNode<CacheItem> GetCacheItem(string key)
    {
        if (Cache.TryGetValue(key, out var cacheItem))
            return cacheItem;

        if (EvictedKeys.Contains(key))
            throw new CacheItemEvictedException(key);

        throw new CacheItemKeyNotFoundException(key);
    }

    protected bool RemoveCacheItem(LinkedListNode<CacheItem> cacheItem)
    {
        if (Cache.TryRemove(cacheItem.Value.CacheKey, out _))
        {
            LruCacheList.Remove(cacheItem);
            return true;
        }

        return false;
    }

    protected void RemoveLeastRecentlyUsedItem()
    {
        var cacheItem = LruCacheList.Last;
        if (cacheItem != null && RemoveCacheItem(cacheItem))
            EvictedKeys.AddFirst(cacheItem.Value.CacheKey);
    }

    protected LinkedListNode<CacheItem> CreateNewCacheItem(string key, object value) =>
          new (new CacheItem(key, value, value.GetType().ToString()));

}
