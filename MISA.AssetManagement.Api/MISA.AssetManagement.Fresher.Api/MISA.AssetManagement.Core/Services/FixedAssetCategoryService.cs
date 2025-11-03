using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Service;

namespace MISA.Core.Services
{
    /// <summary>
    /// Service cho Loại tài sản (FixedAssetCategory)
    /// CreatedBy: HMTuan (29/10/2025)
    /// </summary>
    public class FixedAssetCategoryService
        : BaseService<FixedAssetCategory>, IBaseService<FixedAssetCategory>
    {
        private readonly IFixedAssetCategoryRepository _fixedAssetCategoryrepository;

        /// <summary>
        /// Hàm khởi tạo (constructor) cho lớp FixedAssetCategoryService.
        /// Dùng để gán repository tương ứng và kế thừa logic xử lý từ BaseService.
        /// CreatedBy: HMTuan (29/10/2025)
        /// </summary>
        /// <param name="repository">Đối tượng repository dùng để thao tác dữ liệu của FixedAssetCategory trong tầng dữ liệu.</param>
        public FixedAssetCategoryService(IFixedAssetCategoryRepository repository)
            : base(repository)
        {
            _fixedAssetCategoryrepository = repository;
        }

        /// <summary>
        /// Ghi đè phương thức ValidateBeforeCreate để kiểm tra tính hợp lệ của dữ liệu trước khi thêm mới loại tài sản.
        /// Thực hiện kiểm tra bắt buộc các trường thông tin như mã loại tài sản và tên loại tài sản.
        /// CreatedBy: HMTuan (29/10/2025)
        /// </summary>
        /// <param name="entity">Đối tượng FixedAssetCategory cần được kiểm tra trước khi thêm vào cơ sở dữ liệu.</param>
        /// <exception cref="MISA.Core.Exceptions.ValidateException">Ném ra nếu mã loại tài sản hoặc tên loại tài sản bị để trống.</exception>
        protected override void ValidateBeforeCreate(FixedAssetCategory entity)
        {
            if (string.IsNullOrEmpty(entity.FixedAssetCategoryCode))
                throw new MISA.Core.Exceptions.ValidateException("Mã loại tài sản không được để trống");

            if (string.IsNullOrEmpty(entity.FixedAssetCategoryName))
                throw new MISA.Core.Exceptions.ValidateException("Tên loại tài sản không được để trống");
        }
    }
}
