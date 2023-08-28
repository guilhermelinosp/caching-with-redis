namespace Sample.API.Services
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, DateTimeOffset expirationTime);
        void Remove(string key);
    }
}
