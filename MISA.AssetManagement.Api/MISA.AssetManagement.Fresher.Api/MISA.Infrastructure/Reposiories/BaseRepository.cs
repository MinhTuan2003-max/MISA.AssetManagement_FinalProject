using Dapper;
using MISA.Core.Entities;
using MISA.Core.Interfaces;
using MISA.Core.Interfaces.Repository;
using MySqlConnector;
using System.Text;
using static Dapper.SqlMapper;

namespace MISA.Infrastructure.Reposiories
{
    /// <summary>
    /// Base repository triển khai các phương thức CRUD cơ bản
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam> 
    public abstract class BaseRepository<T> : IBaseRepository<T>
    {
        protected readonly string _connectionString;
        protected readonly string _tableName;

        public BaseRepository(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        /// <summary>
        /// Lấy tên cột code của bảng (convention: <TênBảng>Code)
        /// CreatedBy: HMTuan (30/10/2025)
        /// </summary>
        /// <returns>Tên cột code</returns>
        protected virtual string GetCodeColumnName()
        {
            // Convention: fixed_asset_categories -> fixed_asset_category_code
            // departments -> department_code
            var tableName = _tableName.TrimEnd('s'); // Bỏ 's' cuối
            return $"{tableName}_code";
        }

        /// <summary>
        /// Lấy tất cả bản ghi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <returns>Danh sách entity</returns>
        public virtual IEnumerable<T> GetAll()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = $"SELECT * FROM {_tableName} WHERE is_active = 1 ORDER BY {GetCodeColumnName()} DESC";
                return connection.Query<T>(sql); //Sử dụng queryString
            }
        }

        /// <summary>
        /// Lấy bản ghi theo ID
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Entity hoặc null</returns>
        public virtual T GetById(Guid id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = $"SELECT * FROM {_tableName} WHERE {GetIdColumnName()} = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                return connection.QueryFirstOrDefault<T>(sql, parameters);
            }
        }

        /// <summary>
        /// Thêm mới bản ghi (abstract - implement ở class con)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Số dòng affected</returns>
        public abstract int Insert(T entity);

        /// <summary>
        /// Cập nhật bản ghi (abstract - implement ở class con)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <param name="entity">Entity cần cập nhật</param>
        /// <returns>Số dòng affected</returns>
        public abstract int Update(Guid id, T entity);

        /// <summary>
        /// Xóa bản ghi (soft delete)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Số dòng affected</returns>
        public virtual int Delete(Guid id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = $"DELETE FROM {_tableName} WHERE {GetIdColumnName()} = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id.ToString());
                return connection.Execute(sql, parameters);
            }
        }

        /// <summary>
        /// Xóa nhiều bản ghi (soft delete)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="ids">Danh sách ID</param>
        /// <returns>Số dòng affected</returns>
        public virtual int DeleteMultiple(List<Guid> ids)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var idsString = string.Join(",", ids.Select(id => $"'{id}'"));
                var sql = $"DELETE FROM {_tableName} WHERE {GetIdColumnName()} IN ({idsString})";
                return connection.Execute(sql);
            }
        }

        /// <summary>
        /// Lấy tên cột ID (override trong class con nếu khác)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <returns>Tên cột ID</returns>
        protected virtual string GetIdColumnName()
        {
            return $"{_tableName}_id";
        }
    }
}
