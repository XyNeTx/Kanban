using Newtonsoft.Json;

namespace KANBAN.Services
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private const string JsonContentType = "application/json";

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
                context.Response.ContentType = JsonContentType;
                context.Response.StatusCode = ex.StatusCode;

                var result = JsonConvert.SerializeObject(new
                {
                    status = ex.StatusCode,
                    response = ex.Response,
                    message = ex.InnerException?.Message ?? ex.Message,
                });

                await context.Response.WriteAsync(result);
            }
        }

    }
}
