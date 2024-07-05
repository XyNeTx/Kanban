namespace KANBAN.Models
{
    public class ResponseModel
    {
        public int status { get; set; }
        public string response { get; set; }
        public string title { get; set; }
        public string message { get; set; }
        public string? error { get; set; }
        public object? data { get; set; }
    }
}
