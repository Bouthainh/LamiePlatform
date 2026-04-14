namespace LamiePlatform.DTOs.PlatformDTOs
{
    public class SearchChildDTO
    {
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class SendRequestDTO
    {
        public string ChildId { get; set; } = string.Empty;
    }

    public class RespondToRequestDTO
    {
        public Guid RequestId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}