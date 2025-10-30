using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MISA.Core.Entities
{
    /// <summary>
    /// Thông tin bộ phận sử dụng
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class Department
    {
        /// <summary>
        /// ID của bộ phận sử dụng
        /// </summary>
        public Guid department_id { get; set; }

        /// <summary>
        /// Mã bộ phận sử dụng
        /// </summary>
        public string department_code { get; set; }

        /// <summary>
        /// Tên bộ phận sử dụng
        /// </summary>
        public string department_name { get; set; }

        /// <summary>
        /// Tên viết tắt của bộ phận
        /// CreatedBy: HMTuan (30/10/2025)
        /// </summary>
        public string? department_short_name { get; set; }

        /// <summary>
        /// Mô tả chi tiết về bộ phận
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// Trạng thái hoạt động (true: đang hoạt động, false: đã xóa mềm)
        /// </summary>
        public bool is_active { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime? created_date { get; set; }

        /// <summary>
        /// Người tạo bản ghi
        /// </summary>
        public string? created_by { get; set; }

        /// <summary>
        /// Ngày chỉnh sửa gần nhất
        /// </summary>
        public DateTime? modified_date { get; set; }

        /// <summary>
        /// Người chỉnh sửa gần nhất
        /// </summary>
        public string? modified_by { get; set; }
    }
}
