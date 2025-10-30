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
        public Guid fixed_asset_category_id { get; set; }

        /// <summary>
        /// Mã loại tài sản cố định
        /// </summary>
        public string fixed_asset_category_code { get; set; }

        /// <summary>
        /// Tên loại tài sản cố định
        /// </summary>
        public string fixed_asset_category_name { get; set; }
    }
}
