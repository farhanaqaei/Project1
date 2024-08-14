using Microsoft.Extensions.Caching.Memory;
using Project1.Core.Generals.Interfaces;

namespace Project1.Infrastructure.Cache;

public class CacheManager : ICacheManager
{
    private readonly IMemoryCache _cache;

    public CacheManager(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T Get<T>(string key)
    {
        return _cache.TryGetValue(key, out T value) ? value : default;
    }

    public void Set<T>(string key, T value, TimeSpan expirationTime)
    {
        _cache.Set(key, value, expirationTime);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}