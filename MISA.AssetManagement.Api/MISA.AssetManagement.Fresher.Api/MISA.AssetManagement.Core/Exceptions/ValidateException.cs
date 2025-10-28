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
        public int StatusCode { get; set; }
        public string DevMsg { get; set; }
        public string UserMsg { get; set; }

        public ValidateException(string userMsg, string devMsg = null, int statusCode = 400)
            : base(userMsg)
        {
            UserMsg = userMsg;
            DevMsg = devMsg ?? userMsg;
            StatusCode = statusCode;
        }
    }
}
