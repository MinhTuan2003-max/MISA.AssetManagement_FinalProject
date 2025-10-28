using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs
{
    /// <summary>
    /// DTO lọc và tìm kiếm tài sản
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class FixedAssetFilterDto
    {
        /// <summary>
        /// Từ khóa tìm kiếm (tìm theo tên, mã tài sản)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string? keyword { get; set; }

        /// <summary>
        /// Lọc theo mã bộ phận
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string? department_code { get; set; }

        /// <summary>
        /// Lọc theo mã loại tài sản
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public string? fixed_asset_category_code { get; set; }

        /// <summary>
        /// Số trang (bắt đầu từ 1)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public int page_number { get; set; } = 1;

        /// <summary>
        /// Số bản ghi mỗi trang
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public int page_size { get; set; } = 20;
    }
}
