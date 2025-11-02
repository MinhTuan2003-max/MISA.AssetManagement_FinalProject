using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Exceptions
{
    /// <summary>
    /// Exception cho lỗi not found
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class NotFoundException : ValidateException
    {
        /// <summary>
        /// Khởi tạo ngoại lệ NotFoundException
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="userMsg">Thông báo thân thiện hiển thị cho người dùng.</param>
        /// <param name="devMsg">Thông báo chi tiết dành cho lập trình viên để phục vụ việc debug (tùy chọn).</param>
        public NotFoundException(string userMsg, string devMsg = null)
            : base(userMsg, devMsg, 404) { }
    }
}
