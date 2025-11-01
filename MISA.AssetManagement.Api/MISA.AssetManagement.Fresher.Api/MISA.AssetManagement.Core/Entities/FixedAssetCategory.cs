using System;

namespace MISA.Core.Entities
{
    /// <summary>
    /// Thông tin loại tài sản cố định
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class FixedAssetCategory
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

        /// <summary>
        /// Mô tả thêm về loại tài sản
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
