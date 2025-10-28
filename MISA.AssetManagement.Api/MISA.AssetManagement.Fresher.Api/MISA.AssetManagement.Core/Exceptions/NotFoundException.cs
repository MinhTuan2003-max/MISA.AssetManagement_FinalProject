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
        public NotFoundException(string userMsg, string devMsg = null)
            : base(userMsg, devMsg, 404) { }
    }
}
