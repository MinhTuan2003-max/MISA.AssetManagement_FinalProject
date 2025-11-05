using MISA.Core.Exceptions;
using System.Text.Json;

namespace MISA.AssetManagement.Fresher.Middleware
{
    /// <summary>
    /// Middleware xử lý exception toàn cục trong hệ thống.
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        /// <summary>
        /// Hàm khởi tạo middleware
        /// </summary>
        /// <param name="next">Delegate trỏ tới middleware tiếp theo trong pipeline</param>
        /// <param name="logger">Logger dùng để ghi log lỗi ra console hoặc file</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Phương thức chính để bắt và xử lý mọi exception trong request pipeline.
        /// </summary>
        /// <param name="context">Đối tượng HttpContext chứa thông tin request và response hiện tại</param>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Chuyển tiếp xử lý xuống middleware/controller tiếp theo
                await _next(context);
            }
            catch (ValidateException ex)
            {
                // Log cảnh báo lỗi validate (lỗi do người dùng nhập liệu sai)
                _logger.LogWarning($"[VALIDATE] {ex.DevMsg}");

                // Xử lý lỗi validate (vẫn trả về HTTP 200, success = false)
                await HandleValidateExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                // Log lỗi hệ thống (lỗi không mong muốn)
                _logger.LogError(ex, "Unhandled exception occurred");

                // Xử lý lỗi hệ thống và trả về HTTP 500
                await HandleSystemExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Xử lý ValidateException (lỗi nghiệp vụ, lỗi dữ liệu người dùng nhập sai)
        /// </summary>
        /// <param name="context">Đối tượng HttpContext của request hiện tại</param>
        /// <param name="ex">Đối tượng ValidateException chứa thông tin lỗi</param>
        private static async Task HandleValidateExceptionAsync(HttpContext context, ValidateException ex)
        {
            // Trả về mã 200
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            // Chuẩn hóa object kết quả trả về
            var result = new
            {
                success = false,            // Đánh dấu request thất bại về mặt nghiệp vụ
                statusCode = 200,           // Mã lỗi nghiệp vụ
                userMsg = ex.UserMsg,       // Thông báo thân thiện hiển thị cho người dùng
                devMsg = ex.DevMsg,         // Thông báo chi tiết dành cho developer
                traceId = context.TraceIdentifier // Mã truy vết để debug dễ hơn
            };

            // Chuyển object sang JSON có format camelCase
            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            // Ghi nội dung JSON vào response
            await context.Response.WriteAsync(json);
        }

        /// <summary>
        /// Xử lý các Exception khác (lỗi hệ thống, lỗi không mong muốn)
        /// </summary>
        /// <param name="context">Đối tượng HttpContext của request hiện tại</param>
        /// <param name="ex">Đối tượng Exception chứa thông tin lỗi</param>
        private static async Task HandleSystemExceptionAsync(HttpContext context, Exception ex)
        {
            // Lỗi hệ thống thì trả về HTTP 500
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            // Cấu trúc dữ liệu trả về
            var result = new
            {
                success = false,                     // Đánh dấu request thất bại
                statusCode = 500,                    // Mã lỗi hệ thống
                userMsg = "Đã xảy ra lỗi hệ thống, vui lòng thử lại sau.", // Thông báo người dùng
                devMsg = ex.Message,                 // Thông báo chi tiết cho developer
                traceId = context.TraceIdentifier    // Mã truy vết
            };

            // Chuyển thành JSON format camelCase
            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            // Ghi JSON vào body response
            await context.Response.WriteAsync(json);
        }
    }
}
