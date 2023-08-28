namespace BryanButler.Cache.Exceptions;
public class CacheItemKeyNotFoundException : Exception
{ 
    public CacheItemKeyNotFoundException(string key) : 
        base($"The value for {key} was not available in the cache") { }
}