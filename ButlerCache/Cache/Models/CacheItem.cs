namespace BryanButler.Cache.Models;
public class CacheItem
{
    public string CacheKey { get; }
    public object CacheValue { get; }
    public string CacheType { get; }
    public CacheItem(string key, object value, string type)
    {
        CacheKey = key;
        CacheValue = value;
        CacheType = type;
    }
}