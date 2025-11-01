using System;

namespace MISA.Core.DTOs
{
    /// <summary>
    /// DTO hiển thị thông tin loại tài sản
    /// CreatedBy: HMTuan (29/10/2025)
    /// </summary>
    public class FixedAssetCategoryDto
    {
        /// <summary>
        /// ID của loại tài sản cố định
        /// </summary>
        public Guid FixedAssetCategoryId { get; set; }

        /// <summary>
        /// Mã loại tài sản cố định
        /// </summary>
        public string FixedAssetCategoryCode { get; set; }

        /// <summary>
        /// Tên loại tài sản cố định
        /// </summary>
        public string FixedAssetCategoryName { get; set; }

        /// <summary>
        /// Tên viết tắt cho tài sản cố định
        /// </summary>
        public string? FixedAssetCategoryShortName { get; set; }

        /// <summary>
        /// Thời gian sử dụng (năm)
        /// </summary>
        public int LifeTime { get; set; }

        /// <summary>
        /// Tỷ lệ hao mòn (%)
        /// </summary>
        public decimal DepreciationRate { get; set; }
    }
}
