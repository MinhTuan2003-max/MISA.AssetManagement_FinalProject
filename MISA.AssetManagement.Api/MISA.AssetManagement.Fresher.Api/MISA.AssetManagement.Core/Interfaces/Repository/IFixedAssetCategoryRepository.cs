using MISA.Core.Entities;

namespace MISA.Core.Interfaces.Repository
{

    /// <summary>
    /// Interface repository cho loại tài sản
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public interface IFixedAssetCategoryRepository : IBaseRepository<FixedAssetCategory>
    {
        /// <summary>
        /// Lấy loại tài sản theo mã
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="code">Mã loại tài sản</param>
        /// <returns>Entity loại tài sản</returns>
        FixedAssetCategory GetByCode(string code);
    }

}
