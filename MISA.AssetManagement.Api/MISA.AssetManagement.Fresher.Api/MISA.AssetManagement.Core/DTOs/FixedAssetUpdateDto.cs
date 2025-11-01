using System;

namespace MISA.Core.DTOs
{
    /// <summary>
    /// DTO cập nhật thông tin tài sản
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class FixedAssetUpdateDto
    {
        /// <summary>
        /// Mã tài sản * (bắt buộc)
        /// </summary>
        public string FixedAssetCode { get; set; }

        /// <summary>
        /// Tên tài sản * (bắt buộc)
        /// </summary>
        public string FixedAssetName { get; set; }

        /// <summary>
        /// Mã bộ phận sử dụng * (bắt buộc)
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// Mã loại tài sản * (bắt buộc)
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
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Nguyên giá * (bắt buộc)
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Ngày mua * (bắt buộc)
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// Mô tả (không bắt buộc)
        /// </summary>
        public string? Description { get; set; }
    }
}
