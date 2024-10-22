using Newtonsoft.Json;
using System.Net;

namespace KANBAN.Services
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (CustomHttpException ex)
            {
                string contentType = "application/json";
                int statusCode = ex.Message switch
                {
                    "Data not found" => 404,
                    "Unauthorized" => 401,
                    "Forbidden" => 403,
                    "Bad Request" => 400,
                    _ => 500,
                };

                context.Response.ContentType = contentType;
                context.Response.StatusCode = statusCode;

                var result = JsonConvert.SerializeObject(new
                {
                    status = statusCode,
                    response = "Error",
                    message = ex.Message,
                });

                await context.Response.WriteAsync(result);
            }
        }

    }
}
