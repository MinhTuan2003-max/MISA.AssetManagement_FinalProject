using System;

namespace MISA.Core.Entities
{
    /// <summary>
    /// Thông tin tài sản cố định
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class FixedAsset
    {
        /// <summary>
        /// ID của tài sản cố định
        /// </summary>
        public Guid FixedAssetId { get; set; }

        /// <summary>
        /// Mã tài sản cố định
        /// </summary>
        public string FixedAssetCode { get; set; }

        /// <summary>
        /// Tên tài sản cố định
        /// </summary>
        public string FixedAssetName { get; set; }

        /// <summary>
        /// ID của bộ phận sử dụng
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Mã bộ phận sử dụng
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// Tên bộ phận sử dụng
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// ID của loại tài sản
        /// </summary>
        public Guid FixedAssetCategoryId { get; set; }

        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        public string FixedAssetCategoryCode { get; set; }

        /// <summary>
        /// Tên loại tài sản
        /// </summary>
        public string FixedAssetCategoryName { get; set; }

        /// <summary>
        /// Ngày mua tài sản
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// Năm sản xuất của tài sản
        /// </summary>
        public int ProductionYear { get; set; }

        /// <summary>
        /// Năm bắt đầu theo dõi tài sản
        /// </summary>
        public int TrackedYear { get; set; }

        /// <summary>
        /// Thời gian sử dụng (năm)
        /// </summary>
        public int LifeTime { get; set; }

        /// <summary>
        /// Tỷ lệ hao mòn (%)
        /// </summary>
        public decimal DepreciationRate { get; set; }

        /// <summary>
        /// Số lượng tài sản
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Nguyên giá của tài sản
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Giá trị hao mòn hàng năm
        /// </summary>
        public decimal DepreciationValue { get; set; }

        /// <summary>
        /// Mô tả thêm về tài sản
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Trạng thái hoạt động (true: đang hoạt động, false: ngừng sử dụng)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Người tạo bản ghi
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Ngày chỉnh sửa gần nhất
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Người chỉnh sửa gần nhất
        /// </summary>
        public string? ModifiedBy { get; set; }
    }
}
