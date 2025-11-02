using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Exceptions
{
    /// <summary>
    /// Exception cho lỗi validation
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class ValidateException : Exception
    {
        /// <summary>
        ///   Mã lỗi
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// Thông báo cho người phát triển
        /// </summary>
        public string DevMsg { get; set; }
        /// <summary>
        /// Thông báo cho người dùng
        /// </summary>
        public string UserMsg { get; set; }

        /// <summary>
        /// Khởi tạo ngoại lệ mà không có thông tin chi tiết
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="userMsg">Thông báo thân thiện hiển thị cho người dùng.</param>
        /// <param name="devMsg">Thông báo chi tiết dành cho lập trình viên để phục vụ việc debug (tùy chọn).</param>
        public ValidateException(string userMsg, string devMsg = null, int statusCode = 400)
            : base(userMsg)
        {
            UserMsg = userMsg;
            DevMsg = devMsg ?? userMsg;
            StatusCode = statusCode;
        }
    }
}
