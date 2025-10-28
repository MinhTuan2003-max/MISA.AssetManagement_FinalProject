using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Interfaces.Repository
{
    /// <summary>
    /// Interface base cho repository
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// Lấy tất cả bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <returns>Danh sách entity</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Lấy bản ghi theo ID
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Entity hoặc null</returns>
        T GetById(Guid id);

        /// <summary>
        /// Thêm mới bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Số dòng affected</returns>
        int Insert(T entity);

        /// <summary>
        /// Cập nhật bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <param name="entity">Entity cần cập nhật</param>
        /// <returns>Số dòng affected</returns>
        int Update(Guid id, T entity);

        /// <summary>
        /// Xóa bản ghi theo ID
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Số dòng affected</returns>
        int Delete(Guid id);

        /// <summary>
        /// Xóa nhiều bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="ids">Danh sách ID</param>
        /// <returns>Số dòng affected</returns>
        int DeleteMultiple(List<Guid> ids);
    }
}
