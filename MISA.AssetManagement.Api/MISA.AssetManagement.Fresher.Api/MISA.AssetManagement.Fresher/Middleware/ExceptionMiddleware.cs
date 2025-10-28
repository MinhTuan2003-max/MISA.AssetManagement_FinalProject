using MISA.Core.Exceptions;
using System.Text.Json;

namespace MISA.AssetManagement.Fresher.Middleware
{
    /// <summary>
    /// Middleware xử lý exception toàn cục
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Xử lý exception
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="context">HTTP Context</param>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidateException ex)
            {
                await HandleExceptionAsync(context, ex.StatusCode, ex.UserMsg, ex.DevMsg);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, 500, "Đã xảy ra lỗi hệ thống", ex.Message);
            }
        }

        /// <summary>
        /// Xử lý và trả về response lỗi
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="context">HTTP Context</param>
        /// <param name="statusCode">Status code</param>
        /// <param name="userMsg">Thông báo người dùng</param>
        /// <param name="devMsg">Thông báo developer</param>
        private static Task HandleExceptionAsync(HttpContext context, int statusCode, string userMsg, string devMsg)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var result = new
            {
                DevMsg = devMsg,
                UserMsg = userMsg
            };

            var json = JsonSerializer.Serialize(result);
            return context.Response.WriteAsync(json);
        }
    }
}
