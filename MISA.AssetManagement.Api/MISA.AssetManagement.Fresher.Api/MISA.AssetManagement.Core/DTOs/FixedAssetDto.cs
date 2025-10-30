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
        public Guid fixed_asset_id { get; set; }

        /// <summary>
        /// Mã tài sản
        /// </summary>
        public string fixed_asset_code { get; set; }

        /// <summary>
        /// Tên tài sản
        /// </summary>
        public string fixed_asset_name { get; set; }

        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        public string fixed_asset_category_code { get; set; }

        /// <summary>
        /// Loại tài sản
        /// </summary>
        public string fixed_asset_category_name { get; set; }

        /// <summary>
        /// Mã bộ phận sử dụng
        /// </summary>
        public string department_code { get; set; }

        /// <summary>
        /// Bộ phận sử dụng
        /// </summary>
        public string department_name { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// Nguyên giá
        /// </summary>
        public decimal cost { get; set; }

        /// <summary>
        /// Giá trị hao mòn năm (từ form tính sẵn)
        /// CreatedBy: HMTuan (30/10/2025)
        /// </summary>
        public decimal depreciation_value { get; set; }

        /// <summary>
        /// HM/KH lũy kế = depreciation_value
        /// Công thức: accumulated_depreciation = depreciation_value
        /// CreatedBy: HMTuan (30/10/2025)
        /// </summary>
        public decimal accumulated_depreciation { get; set; }

        /// <summary>
        /// Giá trị còn lại
        /// Công thức: remaining_value = cost - depreciation_value
        /// CreatedBy: HMTuan (30/10/2025)
        /// </summary>
        public decimal remaining_value { get; set; }

        /// <summary>
        /// Ngày mua
        /// </summary>
        public DateTime purchase_date { get; set; }

        /// <summary>
        /// Năm sản xuất
        /// </summary>
        public int production_year { get; set; }

        /// <summary>
        /// Năm theo dõi
        /// </summary>
        public int tracked_year { get; set; }

        /// <summary>
        /// Số năm sử dụng
        /// </summary>
        public int life_time { get; set; }

        /// <summary>
        /// Tỷ lệ hao mòn (%)
        /// </summary>
        public decimal depreciation_rate { get; set; }
    }
}
