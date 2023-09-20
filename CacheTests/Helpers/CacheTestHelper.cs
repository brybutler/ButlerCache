using BryanButler.Cache;
using BryanButler.Cache.Models;

namespace CacheTests.Helpers;
public class CacheTestHelper
{
    public string GetUniqueKey() => Guid.NewGuid().ToString();
    public string GetRandomStringValue() => Guid.NewGuid().ToString().Substring(28, 8);
    public int GetRandomIntValue() => new Random().Next(0, 100);

    public List<string> EvictedKeys = new();

    public (string key, string value) AddNewStringItemToCache()
    {
        var cache = ButlerCache.Instance;
        var key = GetUniqueKey();
        var value = GetRandomStringValue();
        cache.Add(key, value);
        return (key, value);
    }

    public (string key, int value) AddNewIntItemToCache()
    {
        var cache = ButlerCache.Instance;
        var key = GetUniqueKey();
        var value = GetRandomIntValue();
        cache.Add(key, value);
        return (key, value);
    }

    public ButlerCache ResetCacheForTest(int capacity = 10)
    {
        var cache = ButlerCache.Instance;
        cache.Clear();
        cache.SetCapacity(capacity);
        cache.OnRemoval += ItemEvictionEvent;
        EvictedKeys.Clear();
        return cache;
    }

    public void ItemEvictionEvent(object? sender, ItemRemoval item)
    {
        EvictedKeys.Add(item.Key);
    }

}