using Microsoft.AspNetCore.Mvc;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Service;

namespace MISA.AssetManagement.Fresher.Controllers
{
    /// <summary>
    /// Controller quản lý bộ phận
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DepartmentsController : BaseController<Department>
    {
        public DepartmentsController(IBaseService<Department> baseService)
            : base(baseService) { }
    }
}
