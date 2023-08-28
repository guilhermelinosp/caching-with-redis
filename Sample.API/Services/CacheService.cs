using StackExchange.Redis;
using System.Text.Json;

namespace Sample.API.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cache;

        public CacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cache = redis.GetDatabase();
        }

        public T? Get<T>(string key)
        {
            var value = _cache.StringGet(key);

            if (!value.IsNull)
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }

        public void Set<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiry = expirationTime - DateTimeOffset.Now;
            var serializedValue = JsonSerializer.Serialize(value);
            _cache.StringSet(key, serializedValue, expiry);
        }

        public void Remove(string key)
        {
            if (_cache.KeyExists(key))
            {
                _cache.KeyDelete(key);
            }
        }
    }
}