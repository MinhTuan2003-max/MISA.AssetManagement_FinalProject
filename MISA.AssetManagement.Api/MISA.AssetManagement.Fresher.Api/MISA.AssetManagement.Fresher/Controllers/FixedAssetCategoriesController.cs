using Microsoft.AspNetCore.Mvc;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Service;

namespace MISA.AssetManagement.Fresher.Controllers
{
    /// <summary>
    /// Controller quản lý loại tài sản
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FixedAssetCategoriesController : BaseController<FixedAssetCategory>
    {
        public FixedAssetCategoriesController(IBaseService<FixedAssetCategory> baseService)
            : base(baseService) { }
    }
}
