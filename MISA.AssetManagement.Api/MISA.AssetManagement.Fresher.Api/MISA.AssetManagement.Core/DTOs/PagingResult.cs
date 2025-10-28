using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs
{
    /// <summary>
    /// DTO kết quả phân trang
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu</typeparam>
    public class PagingResult<T>
    {
        /// <summary>
        /// Danh sách dữ liệu
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        /// Tổng số bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Số trang hiện tại
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Số bản ghi mỗi trang
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số trang
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        public int TotalPages { get; set; }
    }
}
