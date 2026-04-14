namespace LamiePlatform.Services
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string? Data { get; set; }


        public ServiceResult(bool success, string message, string userId = null, string data = null)
        {
            Success = success;
            Message = message;
            UserId = userId;
            Data = data;
        }

    }
}
