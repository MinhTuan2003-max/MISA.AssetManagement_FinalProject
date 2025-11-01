using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Service;

namespace MISA.Core.Services
{
    /// <summary>
    /// Service cho Bộ phận sử dụng (Department)
    /// CreatedBy: HMTuan (29/10/2025)
    /// </summary>
    public class DepartmentService
        : BaseService<Department>, IBaseService<Department>
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentService(IDepartmentRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Ghi đè ValidateBeforeCreate để kiểm tra dữ liệu trước khi thêm
        /// </summary>
        protected override void ValidateBeforeCreate(Department entity)
        {
            if (string.IsNullOrEmpty(entity.DepartmentCode))
                throw new MISA.Core.Exceptions.ValidateException("Mã bộ phận không được để trống");

            if (string.IsNullOrEmpty(entity.DepartmentName))
                throw new MISA.Core.Exceptions.ValidateException("Tên bộ phận không được để trống");
        }
    }
}
