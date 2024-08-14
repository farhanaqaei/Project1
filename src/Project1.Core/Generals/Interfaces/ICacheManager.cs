namespace Project1.Core.Generals.Interfaces;

public interface ICacheManager
{
    T Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expirationTime);
    void Remove(string key);
}
