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
        public Guid fixed_asset_category_id { get; set; }

        /// <summary>
        /// Mã loại tài sản cố định
        /// </summary>
        public string fixed_asset_category_code { get; set; }

        /// <summary>
        /// Tên loại tài sản cố định
        /// </summary>
        public string fixed_asset_category_name { get; set; }

        /// <summary>
        /// Tên viết tắt cho tài sản cố định
        /// </summary>
        public string? fixed_asset_category_short_name { get; set; }

        /// <summary>
        /// Thời gian sử dụng (năm)
        /// </summary>
        public int life_time { get; set; }

        /// <summary>
        /// Tỷ lệ hao mòn (%)
        /// </summary>
        public decimal depreciation_rate { get; set; }

        /// <summary>
        /// Mô tả thêm về loại tài sản
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// Trạng thái hoạt động (true: đang hoạt động, false: ngừng sử dụng)
        /// </summary>
        public bool is_active { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime? created_date { get; set; }

        /// <summary>
        /// Người tạo bản ghi
        /// </summary>
        public string? created_by { get; set; }

        /// <summary>
        /// Ngày chỉnh sửa gần nhất
        /// </summary>
        public DateTime? modified_date { get; set; }

        /// <summary>
        /// Người chỉnh sửa gần nhất
        /// </summary>
        public string? modified_by { get; set; }
    }
}
