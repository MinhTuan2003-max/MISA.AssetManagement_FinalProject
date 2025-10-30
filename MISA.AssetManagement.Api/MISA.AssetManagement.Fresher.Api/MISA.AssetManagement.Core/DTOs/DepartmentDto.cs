namespace MISA.Core.DTOs
{
    /// <summary>
    /// DTO hiển thị thông tin bộ phận
    /// CreatedBy: HMTuan (29/10/2025)
    /// </summary>
    public class DepartmentDto
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
    }
}
