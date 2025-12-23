namespace RoyalVilla_API.Controllers
{
    public class APIResponse<TData>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; } 
        public string Message { get; set; } = string.Empty;
        public TData? Data { get; set; }
        public object? Errors { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
