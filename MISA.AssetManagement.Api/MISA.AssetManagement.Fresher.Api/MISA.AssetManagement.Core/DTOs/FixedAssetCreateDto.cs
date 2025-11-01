using System;

namespace MISA.Core.DTOs
{
    /// <summary>
    /// DTO tạo mới tài sản
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class FixedAssetCreateDto
    {
        /// <summary>
        /// Mã tài sản * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string FixedAssetCode { get; set; }

        /// <summary>
        /// Tên tài sản * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string FixedAssetName { get; set; }

        /// <summary>
        /// Mã bộ phận sử dụng * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// Mã loại tài sản * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string FixedAssetCategoryCode { get; set; }

        /// <summary>
        /// Tỷ lệ hao mòn (%)
        /// </summary>
        public decimal DepreciationRate { get; set; }

        /// <summary>
        /// Giá trị hao mòn hàng năm
        /// </summary>
        public decimal DepreciationValue { get; set; }

        /// <summary>
        /// Số năm sử dụng
        /// </summary>
        public int LifeTime { get; set; }

        /// <summary>
        /// Số lượng * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Nguyên giá * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Ngày mua * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// Mô tả (không bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string? Description { get; set; }
    }
}
