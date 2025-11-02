using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Exceptions
{
    /// <summary>
    /// Exception cho lỗi conflict (trùng dữ liệu)
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class ConflictException : ValidateException
    {
        /// <summary>
        /// Khởi tạo ngoại lệ xung đột dữ liệu (HTTP 409 Conflict).
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="userMsg">Thông báo thân thiện hiển thị cho người dùng.</param>
        /// <param name="devMsg">Thông báo chi tiết dành cho lập trình viên để phục vụ việc debug (tùy chọn).</param>
        public ConflictException(string userMsg, string devMsg = null)
            : base(userMsg, devMsg, 409) { }
    }
}
