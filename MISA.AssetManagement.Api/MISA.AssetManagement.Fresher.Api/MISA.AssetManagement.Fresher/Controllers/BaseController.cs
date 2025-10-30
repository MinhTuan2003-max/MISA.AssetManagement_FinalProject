using Microsoft.AspNetCore.Mvc;
using MISA.Core.Interfaces.Service;

namespace MISA.AssetManagement.Fresher.Controllers
{
    // <summary>
    /// Base controller cho các controller khác kế thừa
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T> : Controller
    {
        protected readonly IBaseService<T> _baseService;

        public BaseController(IBaseService<T> baseService)
        {
            _baseService = baseService;
        }

        /// <summary>
        /// Lấy tất cả bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <returns>200 OK với danh sách</returns>
        [HttpGet]
        public virtual IActionResult GetAll()
        {
            var data = _baseService.GetAll();
            return Ok(data);
        }

        /// <summary>
        /// Lấy bản ghi theo ID
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>200 OK hoặc 404 Not Found</returns>
        [HttpGet("{id}")]
        public virtual IActionResult GetById(Guid id)
        {
            var data = _baseService.GetById(id);
            return Ok(data);
        }

        /// <summary>
        /// Thêm mới bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>201 Created</returns>
        [HttpPost]
        public virtual IActionResult Post([FromBody] T entity)
        {
            var result = _baseService.Create(entity);
            return StatusCode(201, new
            {
                DevMsg = "Thêm mới thành công",
                UserMsg = "Thêm mới thành công",
                Data = entity
            });
        }

        /// <summary>
        /// Cập nhật bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <param name="entity">Entity cần cập nhật</param>
        /// <returns>200 OK</returns>
        [HttpPut("{id}")]
        public virtual IActionResult Put(Guid id, [FromBody] T entity)
        {
            var result = _baseService.Update(id, entity);
            return Ok(new
            {
                DevMsg = "Cập nhật thành công",
                UserMsg = "Cập nhật thành công",
                Data = entity
            });
        }

        /// <summary>
        /// Xóa bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>200 OK</returns>
        [HttpDelete("{id}")]
        public virtual IActionResult Delete(Guid id)
        {
            var result = _baseService.Delete(id);
            return Ok(new
            {
                DevMsg = "Xóa thành công",
                UserMsg = "Xóa thành công"
            });
        }

        /// <summary>
        /// Xóa nhiều bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="ids">Danh sách ID</param>
        /// <returns>200 OK</returns>
        [HttpDelete("batch")]
        public virtual IActionResult DeleteMultiple([FromBody] List<Guid> ids)
        {
            var result = _baseService.DeleteMultiple(ids);
            return Ok(new
            {
                DevMsg = $"Đã xóa {result} bản ghi",
                UserMsg = $"Đã xóa {result} bản ghi"
            });
        }
    }
}
