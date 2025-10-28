using Microsoft.AspNetCore.Mvc;
using MISA.Core.DTOs;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Service;

namespace MISA.AssetManagement.Fresher.Controllers
{
    /// <summary>
    /// Controller quản lý tài sản cố định
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FixedAssetsController : BaseController<FixedAsset>
    {
        private readonly IFixedAssetService _fixedAssetService;

        public FixedAssetsController(IFixedAssetService fixedAssetService)
            : base(fixedAssetService)
        {
            _fixedAssetService = fixedAssetService;
        }

        /// <summary>
        /// Lấy danh sách tài sản có phân trang và lọc
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <param name="department_code">Mã bộ phận</param>
        /// <param name="fixed_asset_category_code">Mã loại tài sản</param>
        /// <param name="page_number">Số trang</param>
        /// <param name="page_size">Số bản ghi/trang</param>
        /// <returns>200 OK với dữ liệu phân trang</returns>
        [HttpGet("filter")]
        public IActionResult GetPaging(
            [FromQuery] string? keyword,
            [FromQuery] string? department_code,
            [FromQuery] string? fixed_asset_category_code,
            [FromQuery] int page_number = 1,
            [FromQuery] int page_size = 20)
        {
            var filter = new FixedAssetFilterDto
            {
                keyword = keyword,
                department_code = department_code,
                fixed_asset_category_code = fixed_asset_category_code,
                page_number = page_number,
                page_size = page_size
            };

            var result = _fixedAssetService.GetPaging(filter);
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới tài sản từ DTO
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="dto">DTO tạo mới</param>
        /// <returns>201 Created</returns>
        [HttpPost("create")]
        public IActionResult CreateFromDto([FromBody] FixedAssetCreateDto dto)
        {
            var result = _fixedAssetService.CreateFromDto(dto);
            return StatusCode(201, new
            {
                DevMsg = "Thêm mới tài sản thành công",
                UserMsg = "Thêm mới tài sản thành công"
            });
        }

        /// <summary>
        /// Cập nhật tài sản từ DTO
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID tài sản</param>
        /// <param name="dto">DTO cập nhật</param>
        /// <returns>200 OK</returns>
        [HttpPut("{id}/update")]
        public IActionResult UpdateFromDto(Guid id, [FromBody] FixedAssetUpdateDto dto)
        {
            var result = _fixedAssetService.UpdateFromDto(id, dto);
            return Ok(new
            {
                DevMsg = "Cập nhật tài sản thành công",
                UserMsg = "Cập nhật tài sản thành công"
            });
        }

        /// <summary>
        /// Nhân bản tài sản
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID tài sản gốc</param>
        /// <returns>201 Created</returns>
        [HttpPost("{id}/duplicate")]
        public IActionResult Duplicate(Guid id)
        {
            var result = _fixedAssetService.Duplicate(id);
            return StatusCode(201, new
            {
                DevMsg = "Nhân bản tài sản thành công",
                UserMsg = "Nhân bản tài sản thành công"
            });
        }

        
    }
}
