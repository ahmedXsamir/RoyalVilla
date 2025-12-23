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


        public static APIResponse<TData> Create(bool success, int statusCode, string message, TData? data = default, object? errors = null)
        {
            return new APIResponse<TData>
            {
                Success = success,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = errors
            };
        }

        public static APIResponse<TData> Ok(TData data, string message = "Request successful") 
               => Create(true, 200, message, data);

        public static APIResponse<TData> CreatedAt(TData data, string message = "Resource created successfully") 
               => Create(true, 201, message, data);

        public static APIResponse<TData> NoContent(string message = "Operation completed successfully") 
               => Create(true, 204, message);

        public static APIResponse<TData> BadRequest(object errors, string message = "Bad request")
            => Create(false, 400, message, errors:errors);

        public static APIResponse<TData> NotFound(object errors, string message = "Resource not found")
            => Create(false, 404, message);

        public static APIResponse<TData> Confilct(string message = "Conflict occurred")
            => Create(false, 409, message);
        
        public static APIResponse<TData> Error(int statusCode, string message, object? errors = null)
            => Create(false, statusCode, message, errors: errors);
    }
}
