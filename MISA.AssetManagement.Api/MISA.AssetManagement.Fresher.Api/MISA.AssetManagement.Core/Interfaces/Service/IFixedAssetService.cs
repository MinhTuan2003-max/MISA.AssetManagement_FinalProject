using MISA.Core.DTOs;
using MISA.Core.Entities;

namespace MISA.Core.Interfaces.Service
{
    /// <summary>
    /// Interface service cho tài sản cố định
    /// </summary>
    public interface IFixedAssetService : IBaseService<FixedAsset>
    {
        /// <summary>
        /// Lấy danh sách tài sản có phân trang và lọc
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="filter">DTO filter</param>
        /// <returns>Kết quả phân trang</returns>
        PagingResult<FixedAssetDto> GetPaging(FixedAssetFilterDto filter);

        /// <summary>
        /// Tạo mới tài sản từ DTO
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="dto">DTO tạo mới</param>
        /// <returns>Số dòng affected</returns>
        int CreateFromDto(FixedAssetCreateDto dto);

        /// <summary>
        /// Cập nhật tài sản từ DTO
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID tài sản</param>
        /// <param name="dto">DTO cập nhật</param>
        /// <returns>Số dòng affected</returns>
        int UpdateFromDto(Guid id, FixedAssetUpdateDto dto);

        /// <summary>
        /// Nhân bản tài sản (copy và tự động tạo mã mới)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID tài sản gốc</param>
        /// <returns>Số dòng affected</returns>
        int Duplicate(Guid id);
    }
}
