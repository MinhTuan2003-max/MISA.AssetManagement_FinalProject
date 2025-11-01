using System;

namespace MISA.Core.DTOs
{
    /// <summary>
    /// DTO hiển thị danh sách tài sản
    /// UpdatedBy: HMTuan (30/10/2025)
    /// </summary>
    public class FixedAssetDto
    {
        /// <summary>
        /// ID tài sản
        /// </summary>
        public Guid FixedAssetId { get; set; }

        /// <summary>
        /// Mã tài sản
        /// </summary>
        public string FixedAssetCode { get; set; }

        /// <summary>
        /// Tên tài sản
        /// </summary>
        public string FixedAssetName { get; set; }

        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        public string FixedAssetCategoryCode { get; set; }

        /// <summary>
        /// Loại tài sản
        /// </summary>
        public string FixedAssetCategoryName { get; set; }

        /// <summary>
        /// Mã bộ phận sử dụng
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// Bộ phận sử dụng
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Nguyên giá
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Giá trị hao mòn năm (từ form tính sẵn)
        /// CreatedBy: HMTuan (30/10/2025)
        /// </summary>
        public decimal DepreciationValue { get; set; }

        /// <summary>
        /// Ngày mua
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// Năm sản xuất
        /// </summary>
        public int ProductionYear { get; set; }

        /// <summary>
        /// Năm theo dõi
        /// </summary>
        public int TrackedYear { get; set; }

        /// <summary>
        /// Số năm sử dụng
        /// </summary>
        public int LifeTime { get; set; }

        /// <summary>
        /// Tỷ lệ hao mòn (%)
        /// </summary>
        public decimal DepreciationRate { get; set; }
    }
}
