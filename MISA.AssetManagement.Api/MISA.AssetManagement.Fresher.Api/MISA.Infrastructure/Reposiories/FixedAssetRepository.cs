using Dapper;
using MISA.Core.DTOs;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MySqlConnector;
using System.Text;

namespace MISA.Infrastructure.Reposiories
{
    /// <summary>
    /// Repository cho tài sản cố định
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class FixedAssetRepository : BaseRepository<FixedAsset>, IFixedAssetRepository
    {
        public FixedAssetRepository(string connectionString)
            : base(connectionString, "fixed_asset") { }

        /// <summary>
        /// Generate mã tài sản ATOMIC (không trùng lặp khi concurrent)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <returns>Mã tài sản mới</returns>
        public string GenerateNewCodeAtomic()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                try
                {
                    // Sử dụng transaction để đảm bảo atomic
                    using (var transaction = connection.BeginTransaction())
                    {
                        // Lock bảng để prevent race condition
                        var lockSql = "SELECT * FROM fixed_asset ORDER BY created_date DESC LIMIT 1 FOR UPDATE";
                        var lastAsset = connection.QueryFirstOrDefault<dynamic>(lockSql, transaction: transaction);

                        string newCode;
                        if (lastAsset == null)
                        {
                            newCode = "TS000001";
                        }
                        else
                        {
                            var lastCode = (string)lastAsset.fixed_asset_code;
                            var numberPart = lastCode.Substring(2); // Cắt "TS" ra

                            if (int.TryParse(numberPart, out int lastNumber))
                            {
                                // Format với 5 chữ số (D6)
                                newCode = $"TS{(lastNumber + 1):D6}";
                            }
                            else
                            {
                                newCode = "TS000001";
                            }
                        }

                        transaction.Commit();
                        return newCode;
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Kiểm tra mã tài sản đã tồn tại
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="code">Mã tài sản</param>
        /// <param name="excludeId">ID cần loại trừ</param>
        /// <returns>True nếu tồn tại</returns>
        public bool CheckCodeExists(string code, Guid? excludeId = null)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = new StringBuilder();
                sql.Append("SELECT COUNT(*) FROM fixed_asset WHERE fixed_asset_code = @Code");

                if (excludeId.HasValue)
                {
                    sql.Append(" AND fixed_asset_id != @ExcludeId");
                }

                var parameters = new DynamicParameters();
                parameters.Add("@Code", code);
                if (excludeId.HasValue)
                {
                    parameters.Add("@ExcludeId", excludeId.Value.ToString());
                }

                var count = connection.ExecuteScalar<int>(sql.ToString(), parameters);
                return count > 0;
            }
        }

        /// <summary>
        /// Lấy tài sản theo mã
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="code">Mã tài sản</param>
        /// <returns>Entity tài sản</returns>
        public FixedAsset GetByCode(string code)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM fixed_asset WHERE fixed_asset_code = @Code AND is_active = 1";
                var parameters = new DynamicParameters();
                parameters.Add("@Code", code);
                return connection.QueryFirstOrDefault<FixedAsset>(sql, parameters);
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả mã tài sản
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <returns>Danh sách mã tài sản</returns>
        public List<string> GetAllAssetCodes()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT fixed_asset_code FROM fixed_asset WHERE is_active = 1 ORDER BY created_date DESC";
                return connection.Query<string>(sql).ToList();
            }
        }

        /// <summary>
        /// Lấy danh sách tài sản có phân trang và lọc
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="filter">DTO filter</param>
        /// <returns>Kết quả phân trang</returns>
        public PagingResult<FixedAssetDto> GetPaging(FixedAssetFilterDto filter)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = new StringBuilder();
                sql.Append("SELECT ");
                sql.Append("fa.fixed_asset_id, ");
                sql.Append("fa.fixed_asset_code, ");
                sql.Append("fa.fixed_asset_name, ");
                sql.Append("fa.department_code, ");
                sql.Append("fa.department_name, ");
                sql.Append("fa.fixed_asset_category_code, ");
                sql.Append("fa.fixed_asset_category_name, ");
                sql.Append("fa.quantity, ");
                sql.Append("fa.cost, ");
                sql.Append("fa.purchase_date, ");
                sql.Append("fa.production_year, ");
                sql.Append("fa.tracked_year, ");
                sql.Append("fa.life_time, ");
                sql.Append("fa.depreciation_rate, ");
                sql.Append("fa.depreciation_value, ");
                sql.Append("fa.depreciation_value AS accumulated_depreciation, ");
                sql.Append("(fa.cost - fa.depreciation_value) AS remaining_value ");
                sql.Append("FROM fixed_asset fa ");
                sql.Append("WHERE fa.is_active = 1 ");

                var parameters = new DynamicParameters();

                // Tìm kiếm theo keyword
                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                {
                    sql.Append("AND (fa.fixed_asset_code LIKE @Keyword OR fa.fixed_asset_name LIKE @Keyword) ");
                    parameters.Add("@Keyword", $"%{filter.Keyword}%");
                }

                // Lọc theo bộ phận
                if (!string.IsNullOrWhiteSpace(filter.DepartmentCode))
                {
                    sql.Append("AND fa.department_code = @DepartmentCode ");
                    parameters.Add("@DepartmentCode", filter.DepartmentCode);
                }

                // Lọc theo loại tài sản
                if (!string.IsNullOrWhiteSpace(filter.FixedAssetCategoryCode))
                {
                    sql.Append("AND fa.fixed_asset_category_code = @CategoryCode ");
                    parameters.Add("@CategoryCode", filter.FixedAssetCategoryCode);
                }

                // Đếm tổng số bản ghi
                var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
                var totalRecords = connection.ExecuteScalar<int>(countSql, parameters);

                // Phân trang
                sql.Append("ORDER BY fa.fixed_asset_code DESC ");
                sql.Append("LIMIT @PageSize OFFSET @Offset");

                var offset = (filter.PageNumber - 1) * filter.PageSize;
                parameters.Add("@PageSize", filter.PageSize);
                parameters.Add("@Offset", offset);

                var data = connection.Query<FixedAssetDto>(sql.ToString(), parameters).ToList();

                return new PagingResult<FixedAssetDto>
                {
                    Data = data,
                    TotalRecords = totalRecords,
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize)
                };
            }
        }

        /// <summary>
        /// Tạo mã tài sản dùng GenerateNewCodeAtomic
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <returns>Mã tài sản mới</returns>
        public string GenerateNewCode()
        {
            // Dùng GenerateNewCodeAtomic()
            return GenerateNewCodeAtomic();
        }

        /// <summary>
        /// Thêm mới tài sản với mã generated từ server
        /// CreatedBy: HMTuan (31/10/2025)
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Số dòng affected</returns>
        public override int Insert(FixedAsset entity)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                try
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        // Generate mã atomic nếu chưa có
                        if (string.IsNullOrEmpty(entity.FixedAssetCode))
                        {
                            entity.FixedAssetCode = GenerateNewCodeAtomic();
                        }

                        var sql = new StringBuilder();
                        sql.Append("INSERT INTO fixed_asset (");
                        sql.Append("fixed_asset_id, fixed_asset_code, fixed_asset_name, ");
                        sql.Append("department_id, department_code, department_name, ");
                        sql.Append("fixed_asset_category_id, fixed_asset_category_code, fixed_asset_category_name, ");
                        sql.Append("purchase_date, production_year, tracked_year, ");
                        sql.Append("life_time, depreciation_rate, quantity, cost, depreciation_value, ");
                        sql.Append("description, is_active, created_date, created_by, modified_date, modified_by) ");
                        sql.Append("VALUES (");
                        sql.Append("@FixedAssetId, @FixedAssetCode, @FixedAssetName, ");
                        sql.Append("@DepartmentId, @DepartmentCode, @DepartmentName, ");
                        sql.Append("@CategoryId, @CategoryCode, @CategoryName, ");
                        sql.Append("@PurchaseDate, @ProductionYear, @TrackedYear, ");
                        sql.Append("@LifeTime, @DepreciationRate, @Quantity, @Cost, @DepreciationValue, ");
                        sql.Append("@Description, @IsActive, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy)");

                        var parameters = new DynamicParameters();
                        parameters.Add("@FixedAssetId", entity.FixedAssetId.ToString());
                        parameters.Add("@FixedAssetCode", entity.FixedAssetCode);
                        parameters.Add("@FixedAssetName", entity.FixedAssetName);
                        parameters.Add("@DepartmentId", entity.DepartmentId.ToString());
                        parameters.Add("@DepartmentCode", entity.DepartmentCode);
                        parameters.Add("@DepartmentName", entity.DepartmentName);
                        parameters.Add("@CategoryId", entity.FixedAssetCategoryId.ToString());
                        parameters.Add("@CategoryCode", entity.FixedAssetCategoryCode);
                        parameters.Add("@CategoryName", entity.FixedAssetCategoryName);
                        parameters.Add("@PurchaseDate", entity.PurchaseDate);
                        parameters.Add("@ProductionYear", entity.ProductionYear);
                        parameters.Add("@TrackedYear", entity.TrackedYear);
                        parameters.Add("@LifeTime", entity.LifeTime);
                        parameters.Add("@DepreciationRate", entity.DepreciationRate);
                        parameters.Add("@Quantity", entity.Quantity);
                        parameters.Add("@Cost", entity.Cost);
                        parameters.Add("@DepreciationValue", entity.DepreciationValue);
                        parameters.Add("@Description", entity.Description);
                        parameters.Add("@IsActive", true);
                        parameters.Add("@CreatedDate", entity.CreatedDate);
                        parameters.Add("@CreatedBy", entity.CreatedBy);
                        parameters.Add("@ModifiedDate", entity.ModifiedDate);
                        parameters.Add("@ModifiedBy", entity.ModifiedBy);

                        var result = connection.Execute(sql.ToString(), parameters, transaction: transaction);

                        transaction.Commit();
                        return result;
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Cập nhật thông tin tài sản
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID tài sản cần cập nhật</param>
        /// <param name="entity">Thông tin tài sản sau khi chỉnh sửa</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public override int Update(Guid id, FixedAsset entity)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = new StringBuilder();
                sql.Append("UPDATE fixed_asset SET ");
                sql.Append("fixed_asset_code = @FixedAssetCode, ");
                sql.Append("fixed_asset_name = @FixedAssetName, ");
                sql.Append("department_id = @DepartmentId, ");
                sql.Append("department_code = @DepartmentCode, ");
                sql.Append("department_name = @DepartmentName, ");
                sql.Append("fixed_asset_category_id = @CategoryId, ");
                sql.Append("fixed_asset_category_code = @CategoryCode, ");
                sql.Append("fixed_asset_category_name = @CategoryName, ");
                sql.Append("purchase_date = @PurchaseDate, ");
                sql.Append("production_year = @ProductionYear, ");
                sql.Append("tracked_year = @TrackedYear, ");
                sql.Append("life_time = @LifeTime, ");
                sql.Append("depreciation_rate = @DepreciationRate, ");
                sql.Append("quantity = @Quantity, ");
                sql.Append("cost = @Cost, ");
                sql.Append("depreciation_value = @DepreciationValue, ");
                sql.Append("description = @Description, ");
                sql.Append("modified_date = @ModifiedDate, ");
                sql.Append("modified_by = @ModifiedBy ");
                sql.Append("WHERE fixed_asset_id = @FixedAssetId AND is_active = 1");

                var parameters = new DynamicParameters();
                parameters.Add("@FixedAssetId", id.ToString());
                parameters.Add("@FixedAssetCode", entity.FixedAssetCode);
                parameters.Add("@FixedAssetName", entity.FixedAssetName);
                parameters.Add("@DepartmentId", entity.DepartmentId.ToString());
                parameters.Add("@DepartmentCode", entity.DepartmentCode);
                parameters.Add("@DepartmentName", entity.DepartmentName);
                parameters.Add("@CategoryId", entity.FixedAssetCategoryId.ToString());
                parameters.Add("@CategoryCode", entity.FixedAssetCategoryCode);
                parameters.Add("@CategoryName", entity.FixedAssetCategoryName);
                parameters.Add("@PurchaseDate", entity.PurchaseDate);
                parameters.Add("@ProductionYear", entity.ProductionYear);
                parameters.Add("@TrackedYear", entity.TrackedYear);
                parameters.Add("@LifeTime", entity.LifeTime);
                parameters.Add("@DepreciationRate", entity.DepreciationRate);
                parameters.Add("@Quantity", entity.Quantity);
                parameters.Add("@Cost", entity.Cost);
                parameters.Add("@DepreciationValue", entity.DepreciationValue);
                parameters.Add("@Description", entity.Description);
                parameters.Add("@ModifiedDate", entity.ModifiedDate);
                parameters.Add("@ModifiedBy", entity.ModifiedBy);


                return connection.Execute(sql.ToString(), parameters);
            }
        }
    }
}