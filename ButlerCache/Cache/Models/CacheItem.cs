namespace BryanButler.Cache.Models;
public class CacheItem
{
    public string CacheKey { get; }
    public object CacheValue { get; }
    public string CacheType { get; }
    public long Order { get; set; }
    public CacheItem(string key, object value, string type, long order)
    {
        CacheKey = key;
        CacheValue = value;
        CacheType = type;
        Order = order;
    }
}