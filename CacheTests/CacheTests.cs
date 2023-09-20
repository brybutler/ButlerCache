using BryanButler.Cache.Exceptions;
using CacheTests.Helpers;
namespace CacheTests;
public class CacheTests: CacheTestHelper
{
    [Fact]
    public void Verify_that_instance_is_a_Singleton()
    {
        var cacheInstance = ResetCacheForTest();
        var (key, testValue) = AddNewStringItemToCache();
        var value = cacheInstance.Get<string>(key);

        Assert.Equal(testValue, value);
    }
    
    [Fact]
    public void Verify_exception_is_thrown_when_wrong_type_is_requested()
    {
        var cache = ResetCacheForTest();
        var (key, _) = AddNewStringItemToCache();

        Assert.Throws<CacheItemTypeIsIncorrectException>(() => cache.Get<int>(key));
    }

    [Fact]
    public void Verify_basic_return_of_value_when_requested()
    {
        var cache = ResetCacheForTest();
        var (key, value) = AddNewIntItemToCache();
        var returnedValue = cache.Get<int>(key);

        Assert.Equal(value, returnedValue);
    }

    [Fact]
    public void Verify_item_can_be_removed_when_requested()
    {
        var cache = ResetCacheForTest();
        var (key, value) = AddNewIntItemToCache();
        var returnedValue = cache.Get<int>(key);
        Assert.Equal(value, returnedValue);

        cache.Remove(key);
        Assert.Throws<CacheItemKeyNotFoundException>(() => cache.Get<string>(key));
    }


    [Fact]
    public void Verify_false_is_returned_when_adding_to_already_existing_key()
    {
        var cache = ResetCacheForTest();
        var (key, value) = AddNewStringItemToCache();
        Assert.False(cache.Add(key, GetRandomStringValue()));

        var returnedValue = cache.Get<string>(key);
        Assert.Equal(value, returnedValue);
    }

    [Fact]
    public void Verify_eviction_when_too_many_items_added()
    {
        var cache = ResetCacheForTest(2);
        
        var (keyLru, _) = AddNewStringItemToCache();
        var (key2, value2) = AddNewIntItemToCache();
        var (key3, value3) = AddNewStringItemToCache();

        Assert.Contains(keyLru, EvictedKeys);
        Assert.Equal(value2, cache.Get<int>(key2));
        Assert.Equal(value3, cache.Get<string>(key3));
    }
    
    [Fact]
    public void Verify_changing_cache_size_removes_item()
    {
        var cache = ResetCacheForTest(3);

        var (keyLru1, _) = AddNewStringItemToCache();
        var (keyLatest, _) = AddNewIntItemToCache();
        var (keyLru2, _) = AddNewIntItemToCache();
        
        // convert to latest by getting
        var valueLatest = cache.Get<int>(keyLatest);

        cache.SetCapacity(1);

        Assert.Contains(keyLru1, EvictedKeys);
        Assert.Contains(keyLru2, EvictedKeys);
        Assert.Equal(valueLatest, cache.Get<int>(keyLatest));
    }
    
    /// <summary>
    /// Verify that when simultaneous threads access the cache that it
    /// still evicts based on LRU policy.
    /// 1) Set cache capacity to 2 items
    /// 2) Add four items from four simultaneous threads, keeping track of the order that they were added.
    /// 3) Assert that the first two cache items were evicted.
    /// 4) Assert that the last two cache items are accessible in the cache
    /// </summary>
    [Fact]
    public void Verify_cache_evicts_keys_thread_safe_when_adding_to_cache()
    {
        var cache = ResetCacheForTest(2);
        
        string[] keyAdded = new string[4];
        string[] valuesAdded = new string[4];

        var num = 0;
        Parallel.ForEach(Enumerable.Range(0, 4).ToArray(), new ParallelOptions
            {
                MaxDegreeOfParallelism = 4
            },
            (thread) =>
            {
                 var (key, value) = AddNewStringItemToCache();

                 keyAdded[num] = key;
                 valuesAdded[num] = value;
                 num++;
            });

        Assert.Contains(keyAdded[0], EvictedKeys);
        Assert.Contains(keyAdded[1], EvictedKeys);
        Assert.Equal(valuesAdded[2], cache.Get<string>(keyAdded[2]));
        Assert.Equal(valuesAdded[3], cache.Get<string>(keyAdded[3]));
    }
}