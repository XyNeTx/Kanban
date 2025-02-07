using System.Net;

namespace KANBAN.Services
{
    public class CustomHttpException : Exception
    {
        public int StatusCode { get; set; }
        public string Response { get; set; }
        public string Message { get; set; }

        public CustomHttpException(int? statusCode, string? message = null, Exception? ex = null) : base(message, ex)
        {
            if (statusCode == null)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
            }
            StatusCode = statusCode.Value;
            Response = StatusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => "Error",
            };
            Message = ex?.InnerException?.Message ?? message ?? ex!.Message;
        }
    }
}
