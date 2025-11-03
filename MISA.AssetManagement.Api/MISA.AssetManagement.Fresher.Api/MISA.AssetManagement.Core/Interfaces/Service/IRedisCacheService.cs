using System;
using System.Threading.Tasks;

namespace MISA.Core.Interfaces.Service
{
    /// <summary>
    /// Interface cho Redis Cache Service
    /// CreatedBy: HMTuan (03/11/2025)
    /// </summary>
    public interface IRedisCacheService
    {
        /// <summary>
        /// Lấy giá trị từ cache (synchronous)
        /// </summary>
        T Get<T>(string key);

        /// <summary>
        /// Lấy giá trị từ cache (asynchronous)
        /// </summary>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Lưu giá trị vào cache với TTL (tùy chọn)
        /// </summary>
        void Set<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Lưu giá trị vào cache asynchronous
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Xóa key từ cache
        /// </summary>
        void Remove(string key);

        /// <summary>
        /// Xóa key từ cache asynchronous
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// Xóa nhiều keys theo pattern (ví dụ: "fixed_asset:*")
        /// </summary>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// Xóa nhiều keys theo pattern asynchronous
        /// </summary>
        Task RemoveByPatternAsync(string pattern);
    }
}
