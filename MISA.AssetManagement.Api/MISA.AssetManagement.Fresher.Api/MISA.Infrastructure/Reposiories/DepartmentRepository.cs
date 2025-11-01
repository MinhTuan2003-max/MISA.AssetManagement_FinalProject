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
    /// Repository cho bộ phận
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(string connectionString)
            : base(connectionString, "department") { }

        /// <summary>
        /// Lấy bộ phận theo mã
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="code">Mã bộ phận</param>
        /// <returns>Entity bộ phận</returns>
        public Department GetByCode(string code)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM department WHERE department_code = @Code AND is_active = 1";
                var parameters = new DynamicParameters();
                parameters.Add("@Code", code);
                return connection.QueryFirstOrDefault<Department>(sql, parameters);
            }
        }

        /// <summary>
        /// Thêm mới bộ phận
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Số dòng affected</returns>
        public override int Insert(Department entity)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = new StringBuilder();
                sql.Append("INSERT INTO department (department_id, department_code, department_name, ");
                sql.Append("description, is_active, created_date, created_by, modified_date, modified_by) ");
                sql.Append("VALUES (@DepartmentId, @DepartmentCode, @DepartmentName, ");
                sql.Append("@Description, @IsActive, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy)");

                var parameters = new DynamicParameters();
                parameters.Add("@DepartmentId", entity.DepartmentId.ToString());
                parameters.Add("@DepartmentCode", entity.DepartmentCode);
                parameters.Add("@DepartmentName", entity.DepartmentName);
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
        /// Cập nhật bộ phận
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <param name="entity">Entity cần cập nhật</param>
        /// <returns>Số dòng affected</returns>
        public override int Update(Guid id, Department entity)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = new StringBuilder();
                sql.Append("UPDATE department SET ");
                sql.Append("department_name = @DepartmentName, ");
                sql.Append("description = @Description, ");
                sql.Append("modified_date = @ModifiedDate, ");
                sql.Append("modified_by = @ModifiedBy ");
                sql.Append("WHERE department_id = @DepartmentId");

                var parameters = new DynamicParameters();
                parameters.Add("@DepartmentId", id.ToString());
                parameters.Add("@DepartmentName", entity.DepartmentName);
                parameters.Add("@Description", entity.Description);
                parameters.Add("@ModifiedDate", entity.ModifiedDate);
                parameters.Add("@ModifiedBy", entity.ModifiedBy);

                return connection.Execute(sql.ToString(), parameters);
            }
        }
    }
}
