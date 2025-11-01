using Dapper;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Infrastructure.Reposiories
{
    /// <summary>
    /// Repository cho loại tài sản
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class FixedAssetCategoryRepository : BaseRepository<FixedAssetCategory>, IFixedAssetCategoryRepository
    {
        public FixedAssetCategoryRepository(string connectionString)
            : base(connectionString, "fixed_asset_category") { }

        /// <summary>
        /// Lấy loại tài sản theo mã
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="code">Mã loại tài sản</param>
        /// <returns>Entity loại tài sản</returns>
        public FixedAssetCategory GetByCode(string code)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM fixed_asset_category WHERE fixed_asset_category_code = @Code AND is_active = 1";
                var parameters = new DynamicParameters();
                parameters.Add("@Code", code);
                return connection.QueryFirstOrDefault<FixedAssetCategory>(sql, parameters);
            }
        }

        /// <summary>
        /// Thêm mới loại tài sản
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Số dòng affected</returns>
        public override int Insert(FixedAssetCategory entity)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = new StringBuilder();
                sql.Append("INSERT INTO fixed_asset_category (fixed_asset_category_id, fixed_asset_category_code, ");
                sql.Append("fixed_asset_category_name, life_time, depreciation_rate, description, is_active, ");
                sql.Append("created_date, created_by, modified_date, modified_by) ");
                sql.Append("VALUES (@CategoryId, @CategoryCode, @CategoryName, @LifeTime, @DepreciationRate, ");
                sql.Append("@Description, @IsActive, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy)");

                var parameters = new DynamicParameters();
                parameters.Add("@CategoryId", entity.FixedAssetCategoryId.ToString());
                parameters.Add("@CategoryCode", entity.FixedAssetCategoryCode);
                parameters.Add("@CategoryName", entity.FixedAssetCategoryName);
                parameters.Add("@LifeTime", entity.LifeTime);
                parameters.Add("@DepreciationRate", entity.DepreciationRate);
                parameters.Add("@Description", entity.Description);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@CreatedDate", entity.CreatedDate);
                parameters.Add("@CreatedBy", entity.CreatedBy);
                parameters.Add("@ModifiedDate", entity.ModifiedDate);
                parameters.Add("@ModifiedBy", entity.ModifiedBy);

                return connection.Execute(sql.ToString(), parameters);
            }
        }

        /// <summary>
        /// Cập nhật loại tài sản
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <param name="entity">Entity cần cập nhật</param>
        /// <returns>Số dòng affected</returns>
        public override int Update(Guid id, FixedAssetCategory entity)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = new StringBuilder();
                sql.Append("UPDATE fixed_asset_category SET ");
                sql.Append("fixed_asset_category_name = @CategoryName, ");
                sql.Append("life_time = @LifeTime, ");
                sql.Append("depreciation_rate = @DepreciationRate, ");
                sql.Append("description = @Description, ");
                sql.Append("modified_date = @ModifiedDate, ");
                sql.Append("modified_by = @ModifiedBy ");
                sql.Append("WHERE fixed_asset_category_id = @CategoryId");

                var parameters = new DynamicParameters();
                parameters.Add("@CategoryId", id.ToString());
                parameters.Add("@CategoryName", entity.FixedAssetCategoryName);
                parameters.Add("@LifeTime", entity.LifeTime);
                parameters.Add("@DepreciationRate", entity.DepreciationRate);
                parameters.Add("@Description", entity.Description);
                parameters.Add("@ModifiedDate", entity.ModifiedDate);
                parameters.Add("@ModifiedBy", entity.ModifiedBy);

                return connection.Execute(sql.ToString(), parameters);
            }
        }
    }
}
