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
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Mã bộ phận sử dụng
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// Tên bộ phận sử dụng
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Tên viết tắt của bộ phận
        /// CreatedBy: HMTuan (30/10/2025)
        /// </summary>
        public string? DepartmentShortName { get; set; }

        /// <summary>
        /// Mô tả chi tiết về bộ phận
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Trạng thái hoạt động (true: đang hoạt động, false: đã xóa mềm)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Người tạo bản ghi
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Ngày chỉnh sửa gần nhất
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Người chỉnh sửa gần nhất
        /// </summary>
        public string? ModifiedBy { get; set; }
    }
}
