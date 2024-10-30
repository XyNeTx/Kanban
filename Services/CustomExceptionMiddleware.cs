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

                context.Response.ContentType = contentType;
                context.Response.StatusCode = ex.StatusCode;

                var result = JsonConvert.SerializeObject(new
                {
                    status = ex.StatusCode,
                    response = ex.Response,
                    message = ex.Message,
                });

                await context.Response.WriteAsync(result);
            }
        }

    }
}
