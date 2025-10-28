using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs
{
    /// <summary>
    /// DTO cập nhật tài sản
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class FixedAssetUpdateDto
    {
        /// <summary>
        /// Má tài liệu
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string fixed_asset_name { get; set; }
        /// <summary>
        /// Mã phòng ban
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string department_code { get; set; }
        /// <summary>
        /// Mã loại tài liệu
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string fixed_asset_category_code { get; set; }
        /// <summary>
        /// Số lượng
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// Giá tài liệu
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public decimal cost { get; set; }
        /// <summary>
        /// Ngày mua
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public DateTime purchase_date { get; set; }
        /// <summary>
        /// Mô tả
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string? description { get; set; }
    }
}
