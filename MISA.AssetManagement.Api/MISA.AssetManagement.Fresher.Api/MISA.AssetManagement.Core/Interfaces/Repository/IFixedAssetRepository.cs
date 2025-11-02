using MISA.Core.DTOs;
using MISA.Core.Entities;

namespace MISA.Core.Interfaces.Repository
{
    /// <summary>
    /// Interface repository cho tài sản cố định
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public interface IFixedAssetRepository : IBaseRepository<FixedAsset>
    {
        /// <summary>
        /// Kiểm tra mã tài sản đã tồn tại
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="code">Mã tài sản</param>
        /// <param name="excludeId">ID cần loại trừ (dùng khi update)</param>
        /// <returns>True nếu tồn tại</returns>
        bool CheckCodeExists(string code, Guid? excludeId = null);

        /// <summary>
        /// Lấy tài sản theo mã
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="code">Mã tài sản</param>
        /// <returns>Entity tài sản</returns>
        FixedAsset GetByCode(string code);

        /// <summary>
        /// Lấy danh sách tài sản có phân trang và lọc
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="filter">DTO filter</param>
        /// <returns>Kết quả phân trang</returns>
        PagingResult<FixedAssetDto> GetPaging(FixedAssetFilterDto filter);

        /// <summary>
        /// Tạo mã tài sản tự động tăng
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <returns>Mã tài sản mới</returns>
        string GenerateNewCode();
    }
}
