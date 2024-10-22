using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace KANBAN.Services
{
    public class CustomHttpException : Exception
    {

        public CustomHttpException(string message) : base(message)
        {
        }
    }
}
