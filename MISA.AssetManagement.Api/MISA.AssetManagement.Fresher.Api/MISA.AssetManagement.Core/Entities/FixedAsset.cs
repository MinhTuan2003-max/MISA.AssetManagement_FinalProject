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
        public Guid fixed_asset_id { get; set; }

        /// <summary>
        /// Mã tài sản cố định
        /// </summary>
        public string fixed_asset_code { get; set; }

        /// <summary>
        /// Tên tài sản cố định
        /// </summary>
        public string fixed_asset_name { get; set; }

        /// <summary>
        /// ID của bộ phận sử dụng
        /// </summary>
        public Guid department_id { get; set; }

        /// <summary>
        /// Mã bộ phận sử dụng
        /// </summary>
        public string department_code { get; set; }

        /// <summary>
        /// Tên bộ phận sử dụng
        /// </summary>
        public string department_name { get; set; }

        /// <summary>
        /// ID của loại tài sản
        /// </summary>
        public Guid fixed_asset_category_id { get; set; }

        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        public string fixed_asset_category_code { get; set; }

        /// <summary>
        /// Tên loại tài sản
        /// </summary>
        public string fixed_asset_category_name { get; set; }

        /// <summary>
        /// Ngày mua tài sản
        /// </summary>
        public DateTime purchase_date { get; set; }

        /// <summary>
        /// Năm sản xuất của tài sản
        /// </summary>
        public int production_year { get; set; }

        /// <summary>
        /// Năm bắt đầu theo dõi tài sản
        /// </summary>
        public int tracked_year { get; set; }

        /// <summary>
        /// Thời gian sử dụng (năm)
        /// </summary>
        public int life_time { get; set; }

        /// <summary>
        /// Tỷ lệ hao mòn (%)
        /// </summary>
        public decimal depreciation_rate { get; set; }

        /// <summary>
        /// Số lượng tài sản
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// Nguyên giá của tài sản
        /// </summary>
        public decimal cost { get; set; }

        /// <summary>
        /// Giá trị hao mòn hàng năm
        /// </summary>
        public decimal depreciation_value { get; set; }

        /// <summary>
        /// Mô tả thêm về tài sản
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
