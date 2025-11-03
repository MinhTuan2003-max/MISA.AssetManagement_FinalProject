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
        private readonly IDepartmentRepository _departmentRepository;

        /// <summary>
        /// Hàm khởi tạo (constructor) cho lớp DepartmentService.
        /// Dùng để gán repository tương ứng và kế thừa logic xử lý từ BaseService.
        /// CreatedBy: HMTuan (29/10/2025)
        /// </summary>
        /// <param name="repository">Đối tượng repository dùng để thao tác dữ liệu của Department trong tầng dữ liệu.</param>
        public DepartmentService(IDepartmentRepository repository)
            : base(repository)
        {
            _departmentRepository = repository;
        }

        /// <summary>
        /// Ghi đè ValidateBeforeCreate nếu cần kiểm tra dữ liệu trước khi thêm
        /// CreatedBy: HMTuan (29/10/2025)
        /// </summary>
        /// <param name="entity">Đối tượng Department cần được kiểm tra trước khi thêm vào cơ sở dữ liệu.</param>
        /// <exception cref="MISA.Core.Exceptions.ValidateException"></exception>
        protected override void ValidateBeforeCreate(Department entity)
        {
            if (string.IsNullOrEmpty(entity.DepartmentCode))
                throw new MISA.Core.Exceptions.ValidateException("Mã bộ phận không được để trống");

            if (string.IsNullOrEmpty(entity.DepartmentName))
                throw new MISA.Core.Exceptions.ValidateException("Tên bộ phận không được để trống");
        }
    }
}
