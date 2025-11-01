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
        private readonly IFixedAssetCategoryRepository _repository;

        public FixedAssetCategoryService(IFixedAssetCategoryRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Ghi đè ValidateBeforeCreate nếu cần kiểm tra dữ liệu trước khi thêm
        /// </summary>
        protected override void ValidateBeforeCreate(FixedAssetCategory entity)
        {
            if (string.IsNullOrEmpty(entity.FixedAssetCategoryCode))
                throw new MISA.Core.Exceptions.ValidateException("Mã loại tài sản không được để trống");

            if (string.IsNullOrEmpty(entity.FixedAssetCategoryName))
                throw new MISA.Core.Exceptions.ValidateException("Tên loại tài sản không được để trống");
        }
    }
}
