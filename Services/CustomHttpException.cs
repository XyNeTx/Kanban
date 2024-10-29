using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace KANBAN.Services
{
    public class CustomHttpException : Exception
    {
        public int StatusCode { get; set; }

        public CustomHttpException(int? statusCode, string message) : base(message)
        {
            if (statusCode == null)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
            }
            StatusCode = statusCode.Value;
        }
    }
}
