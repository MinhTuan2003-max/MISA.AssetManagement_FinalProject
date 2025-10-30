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
                if (!string.IsNullOrWhiteSpace(filter.keyword))
                {
                    sql.Append("AND (fa.fixed_asset_code LIKE @Keyword OR fa.fixed_asset_name LIKE @Keyword) ");
                    parameters.Add("@Keyword", $"%{filter.keyword}%");
                }

                // Lọc theo bộ phận
                if (!string.IsNullOrWhiteSpace(filter.department_code))
                {
                    sql.Append("AND fa.department_code = @DepartmentCode ");
                    parameters.Add("@DepartmentCode", filter.department_code);
                }

                // Lọc theo loại tài sản
                if (!string.IsNullOrWhiteSpace(filter.fixed_asset_category_code))
                {
                    sql.Append("AND fa.fixed_asset_category_code = @CategoryCode ");
                    parameters.Add("@CategoryCode", filter.fixed_asset_category_code);
                }

                // Đếm tổng số bản ghi
                var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
                var totalRecords = connection.ExecuteScalar<int>(countSql, parameters);

                // Phân trang
                var offset = (filter.page_number - 1) * filter.page_size;
                sql.Append("ORDER BY fa.created_date DESC ");
                sql.Append($"LIMIT {filter.page_size} OFFSET {offset}");

                var data = connection.Query<FixedAssetDto>(sql.ToString(), parameters).ToList();

                return new PagingResult<FixedAssetDto>
                {
                    Data = data,
                    TotalRecords = totalRecords,
                    CurrentPage = filter.page_number,
                    PageSize = filter.page_size,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / filter.page_size)
                };
            }
        }


        /// <summary>
        /// Tạo mã tài sản tự động tăng
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <returns>Mã tài sản mới</returns>
        public string GenerateNewCode()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT fixed_asset_code FROM fixed_asset ORDER BY created_date DESC LIMIT 1";
                var lastCode = connection.QueryFirstOrDefault<string>(sql);

                if (string.IsNullOrEmpty(lastCode))
                {
                    return "TS00001";
                }

                // Lấy phần số từ mã (ví dụ: TS001 -> 001)
                var numberPart = lastCode.Substring(2);
                var nextNumber = int.Parse(numberPart) + 1;
                return $"TS{nextNumber:D5}";
            }
        }

        /// <summary>
        /// Tính giá trị còn lại của tài sản (sử dụng stored procedure)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="fixedAssetId">ID tài sản</param>
        /// <returns>Giá trị còn lại</returns>
        public decimal CalculateRemainingValue(Guid fixedAssetId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_fixed_asset_id", fixedAssetId.ToString());
                parameters.Add("@p_remaining_value", dbType: System.Data.DbType.Decimal, direction: System.Data.ParameterDirection.Output);

                connection.Execute("sp_calculate_remaining_value", parameters, commandType: System.Data.CommandType.StoredProcedure);

                return parameters.Get<decimal>("@p_remaining_value");
            }
        }

        /// <summary>
        /// Thêm mới tài sản
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Số dòng affected</returns>
        public override int Insert(FixedAsset entity)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
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
                parameters.Add("@FixedAssetId", entity.fixed_asset_id.ToString());
                parameters.Add("@FixedAssetCode", entity.fixed_asset_code);
                parameters.Add("@FixedAssetName", entity.fixed_asset_name);
                parameters.Add("@DepartmentId", entity.department_id.ToString());
                parameters.Add("@DepartmentCode", entity.department_code);
                parameters.Add("@DepartmentName", entity.department_name);
                parameters.Add("@CategoryId", entity.fixed_asset_category_id.ToString());
                parameters.Add("@CategoryCode", entity.fixed_asset_category_code);
                parameters.Add("@CategoryName", entity.fixed_asset_category_name);
                parameters.Add("@PurchaseDate", entity.purchase_date);
                parameters.Add("@ProductionYear", entity.production_year);
                parameters.Add("@TrackedYear", entity.tracked_year);
                parameters.Add("@LifeTime", entity.life_time);
                parameters.Add("@DepreciationRate", entity.depreciation_rate);
                parameters.Add("@Quantity", entity.quantity);
                parameters.Add("@Cost", entity.cost);
                parameters.Add("@DepreciationValue", entity.depreciation_value);
                parameters.Add("@Description", entity.description);
                parameters.Add("@IsActive", true);
                parameters.Add("@CreatedDate", entity.created_date);
                parameters.Add("@CreatedBy", entity.created_by);
                parameters.Add("@ModifiedDate", entity.modified_date);
                parameters.Add("@ModifiedBy", entity.modified_by);

                return connection.Execute(sql.ToString(), parameters);
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
                parameters.Add("@FixedAssetCode", entity.fixed_asset_code);
                parameters.Add("@FixedAssetName", entity.fixed_asset_name);
                parameters.Add("@DepartmentId", entity.department_id.ToString());
                parameters.Add("@DepartmentCode", entity.department_code);
                parameters.Add("@DepartmentName", entity.department_name);
                parameters.Add("@CategoryId", entity.fixed_asset_category_id.ToString());
                parameters.Add("@CategoryCode", entity.fixed_asset_category_code);
                parameters.Add("@CategoryName", entity.fixed_asset_category_name);
                parameters.Add("@PurchaseDate", entity.purchase_date);
                parameters.Add("@ProductionYear", entity.production_year);
                parameters.Add("@TrackedYear", entity.tracked_year);
                parameters.Add("@LifeTime", entity.life_time);
                parameters.Add("@DepreciationRate", entity.depreciation_rate);
                parameters.Add("@Quantity", entity.quantity);
                parameters.Add("@Cost", entity.cost);
                parameters.Add("@DepreciationValue", entity.depreciation_value);
                parameters.Add("@Description", entity.description);
                parameters.Add("@ModifiedDate", entity.modified_date);
                parameters.Add("@ModifiedBy", entity.modified_by);

                return connection.Execute(sql.ToString(), parameters);
            }
        }
    }
}

