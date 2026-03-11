namespace BadeePlatform.Services
{
    public interface IRequestService
    {
        Task<List<Models.Child>> SearchChildrenAsync(string searchTerm, string educatorId);
        Task<(bool Success, string Message)> SendRequestAsync(string educatorId, string childId);
        Task<List<Models.Request>> GetPendingRequestsByParentAsync(string parentId);
        Task<(bool Success, string Message)> RespondToRequestAsync(Guid requestId, string status, string parentId);
        Task<List<Models.Request>> GetSentRequestsByEducatorAsync(string educatorId);
        Task<List<Models.Child>> GetEducatorStudentsAsync(string educatorId);
    }
}