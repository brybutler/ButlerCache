namespace BryanButler.Cache.Exceptions;
public class CacheItemTypeIsIncorrectException : Exception
{
    public CacheItemTypeIsIncorrectException(string key, string actualType, string requestedType) :
        base($"The value for {key} is a {actualType} not a {requestedType}")
    { }
}