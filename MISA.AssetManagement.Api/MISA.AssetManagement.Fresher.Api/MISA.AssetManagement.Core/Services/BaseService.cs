using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Services
{
    using MISA.Core.Exceptions;
    using MISA.Core.Interfaces;
    using MISA.Core.Interfaces.Repository;
    using MISA.Core.Interfaces.Service;

    /// <summary>
    /// Base service triển khai các phương thức CRUD cơ bản
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public abstract class BaseService<T> : IBaseService<T>
    {
        protected readonly IBaseRepository<T> _repository;

        public BaseService(IBaseRepository<T> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Lấy tất cả bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <returns>Danh sách entity</returns>
        public virtual IEnumerable<T> GetAll()
        {
            return _repository.GetAll();
        }

        /// <summary>
        /// Lấy bản ghi theo ID
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Entity</returns>
        public virtual T GetById(Guid id)
        {
            var entity = _repository.GetById(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy bản ghi với ID: {id}");
            }
            return entity;
        }

        /// <summary>
        /// Thêm mới bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Số dòng affected</returns>
        public virtual int Create(T entity)
        {
            // Validate trước khi thêm
            ValidateBeforeCreate(entity);

            var result = _repository.Insert(entity);
            if (result <= 0)
            {
                throw new Exception("Không thể thêm mới bản ghi");
            }
            return result;
        }

        /// <summary>
        /// Cập nhật bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <param name="entity">Entity cần cập nhật</param>
        /// <returns>Số dòng affected</returns>
        public virtual int Update(Guid id, T entity)
        {
            // Kiểm tra tồn tại
            GetById(id);

            // Validate trước khi cập nhật
            ValidateBeforeUpdate(id, entity);

            var result = _repository.Update(id, entity);
            if (result <= 0)
            {
                throw new Exception("Không thể cập nhật bản ghi");
            }
            return result;
        }

        /// <summary>
        /// Xóa bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Số dòng affected</returns>
        public virtual int Delete(Guid id)
        {
            // Kiểm tra tồn tại
            GetById(id);

            var result = _repository.Delete(id);
            if (result <= 0)
            {
                throw new Exception("Không thể xóa bản ghi");
            }
            return result;
        }

        /// <summary>
        /// Xóa nhiều bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="ids">Danh sách ID</param>
        /// <returns>Số dòng affected</returns>
        public virtual int DeleteMultiple(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                throw new ValidateException("Danh sách ID không được rỗng");
            }

            var result = _repository.DeleteMultiple(ids);
            return result;
        }

        /// <summary>
        /// Validate trước khi tạo mới (override trong class con)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="entity">Entity cần validate</param>
        protected virtual void ValidateBeforeCreate(T entity) { }

        /// <summary>
        /// Validate trước khi cập nhật (override trong class con)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID entity</param>
        /// <param name="entity">Entity cần validate</param>
        protected virtual void ValidateBeforeUpdate(Guid id, T entity) { }
    }
}
