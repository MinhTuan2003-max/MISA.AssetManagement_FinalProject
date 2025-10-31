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
        /// Mã tài sản * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string fixed_asset_code { get; set; }

        /// <summary>
        /// Tên tài sản * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string fixed_asset_name { get; set; }

        /// <summary>
        /// Mã bộ phận sử dụng * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string department_code { get; set; }

        /// <summary>
        /// Mã loại tài sản * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string fixed_asset_category_code { get; set; }

        /// <summary>
        /// Tỷ lệ hao mòn (%)
        /// </summary>
        public decimal depreciation_rate { get; set; }

        /// <summary>
        /// Giá trị hao mòn hàng năm
        /// </summary>
        public decimal depreciation_value { get; set; }

        /// <summary>
        /// Số năm sử dụng
        /// </summary>
        public int life_time { get; set; }

        /// <summary>
        /// Số lượng * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// Nguyên giá * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public decimal cost { get; set; }

        /// <summary>
        /// Ngày mua * (bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public DateTime purchase_date { get; set; }

        /// <summary>
        /// Mô tả (không bắt buộc)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string? description { get; set; }
    }
}
