using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs
{
    /// <summary>
    /// DTO hiển thị danh sách tài sản
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class FixedAssetDto
    {
        /// <summary>
        /// ID tài sản
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public Guid fixed_asset_id { get; set; }

        /// <summary>
        /// Mã tài sản
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string fixed_asset_code { get; set; }

        /// <summary>
        /// Tên tài sản
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string fixed_asset_name { get; set; }

        /// <summary>
        /// Loại tài sản
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string fixed_asset_category_name { get; set; }

        /// <summary>
        /// Bộ phận sử dụng
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string department_name { get; set; }

        /// <summary>
        /// Số lượng
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// Nguyên giá
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public decimal cost { get; set; }

        /// <summary>
        /// HM/KH lũy kế (Hao mòn/Khấu hao lũy kế)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public decimal accumulated_depreciation { get; set; }

        /// <summary>
        /// Giá trị còn lại
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public decimal remaining_value { get; set; }
    }
}
