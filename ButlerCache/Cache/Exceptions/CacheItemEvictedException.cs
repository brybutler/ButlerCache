namespace BryanButler.Cache.Exceptions;
public class CacheItemEvictedException : Exception
{ 
    public CacheItemEvictedException(string key) : 
        base($"The value for {key} has been evicted from the cache") { }
}