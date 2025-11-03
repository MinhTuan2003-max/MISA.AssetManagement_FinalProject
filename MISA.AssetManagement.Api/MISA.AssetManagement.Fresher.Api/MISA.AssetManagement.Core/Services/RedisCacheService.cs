using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MISA.Core.Interfaces.Service;

namespace MISA.Core.Services
{
    /// <summary>
    /// Service xử lý Redis cache
    /// CreatedBy: HMTuan (03/11/2025)
    /// </summary>
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _db;
        private readonly IServer _server;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
            var endpoints = redis.GetEndPoints();
            _server = redis.GetServer(endpoints.First());
        }

        /// <summary>
        /// Lấy giá trị từ cache (synchronous)
        /// </summary>
        public T Get<T>(string key)
        {
            try
            {
                var value = _db.StringGet(key);
                if (!value.HasValue)
                    return default;

                return JsonConvert.DeserializeObject<T>(value.ToString());
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                Console.WriteLine($"Cache GET error for key {key}: {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// Lấy giá trị từ cache (asynchronous)
        /// </summary>
        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);
                if (!value.HasValue)
                    return default;

                return JsonConvert.DeserializeObject<T>(value.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cache GET error for key {key}: {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// Lưu giá trị vào cache với TTL (tùy chọn)
        /// </summary>
        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var json = JsonConvert.SerializeObject(value);
                _db.StringSet(key, json, expiration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cache SET error for key {key}: {ex.Message}");
            }
        }

        /// <summary>
        /// Lưu giá trị vào cache asynchronous
        /// </summary>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var json = JsonConvert.SerializeObject(value);
                await _db.StringSetAsync(key, json, expiration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cache SET error for key {key}: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa key từ cache
        /// </summary>
        public void Remove(string key)
        {
            try
            {
                _db.KeyDelete(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cache DELETE error for key {key}: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa key từ cache asynchronous
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            try
            {
                await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cache DELETE error for key {key}: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa nhiều keys theo pattern (ví dụ: "fixed_asset:*")
        /// </summary>
        public void RemoveByPattern(string pattern)
        {
            try
            {
                var keys = _server.Keys(pattern: pattern);
                if (keys.Any())
                {
                    _db.KeyDelete(keys.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cache DELETE BY PATTERN error for pattern {pattern}: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa nhiều keys theo pattern asynchronous
        /// </summary>
        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var keys = _server.Keys(pattern: pattern);
                if (keys.Any())
                {
                    await _db.KeyDeleteAsync(keys.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cache DELETE BY PATTERN error for pattern {pattern}: {ex.Message}");
            }
        }
    }
}
