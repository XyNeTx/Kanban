using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace KANBAN.Services
{
    public class CustomHttpException : Exception
    {
        public int StatusCode { get; set; }
        public string Response { get; set; }

        public CustomHttpException(int? statusCode, string message) : base(message)
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
        }
    }
}
