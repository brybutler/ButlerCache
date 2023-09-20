using BryanButler.Cache.Exceptions;
using BryanButler.Cache.Models;
namespace BryanButler.Cache.Services;
public class CacheService
{
    protected Dictionary<string, CacheItem> CacheDictionary = new();
    public event EventHandler<ItemRemoval>? OnRemoval;

    private long GetNewOrderNumber()
    {
        if (CacheDictionary.Any())
            return CacheDictionary.Max(_ => _.Value.Order) + 1;

        return 1;
    } 

    protected CacheItem GetCacheItem(string key)
    {
        if (CacheDictionary.TryGetValue(key, out var cacheItem))
        {
            cacheItem.Order = GetNewOrderNumber();
            return cacheItem;
        }

        throw new CacheItemKeyNotFoundException(key);
    }

    protected bool RemoveByKey(string key) => CacheDictionary.Remove(key);

    protected void RemoveLeastRecentlyUsedItem()
    {
        var cacheItem = CacheDictionary.Values.MinBy(_ => _.Order);
        if (cacheItem != null && RemoveByKey(cacheItem.CacheKey) && OnRemoval != null)
            OnRemoval.Invoke(this, new ItemRemoval(cacheItem.CacheKey, cacheItem.CacheType));
    }

    protected CacheItem CreateNewCacheItem(string key, object value) => new (key, value, value.GetType().ToString(), GetNewOrderNumber());

}
