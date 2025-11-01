using System;

namespace MISA.Core.DTOs
{
    /// <summary>
    /// DTO lọc và tìm kiếm tài sản
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class FixedAssetFilterDto
    {
        /// <summary>
        /// Từ khóa tìm kiếm (tìm theo tên, mã tài sản)
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Lọc theo mã bộ phận
        /// </summary>
        public string? DepartmentCode { get; set; }

        /// <summary>
        /// Lọc theo mã loại tài sản
        /// </summary>
        public string? FixedAssetCategoryCode { get; set; }

        /// <summary>
        /// Số trang (bắt đầu từ 1)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số bản ghi mỗi trang
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
