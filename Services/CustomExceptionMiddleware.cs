using Newtonsoft.Json;

namespace KANBAN.Services
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private const string JsonContentType = "application/json";

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
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
                    message = ex.Message,
                });

                await context.Response.WriteAsync(result);
            }
            //catch (Exception ex)
            //{
            //    context.Response.ContentType = JsonContentType;
            //    context.Response.StatusCode = 500;

            //    var result = JsonConvert.SerializeObject(new
            //    {
            //        status = 500,
            //        response = "Internal Server Error",
            //        message = ex.Message,
            //    });

            //    await context.Response.WriteAsync(result);
            //}
        }

    }
}
