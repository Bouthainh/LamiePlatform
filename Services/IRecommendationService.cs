using LamiePlatform.DTOs.PlatformDTOs;
using LamiePlatform.Models.ViewModels;
using LamiePlatform.Models.ViewModels.DashboardViewModels;
using LamiePlatform.Services;


namespace LamiePlatform.Services
{
    public interface IRecommendationService
    {
        Task<ServiceResult> GenerateAndSaveAsync(string childId, string? educatorId = null);
        Task<List<RecommendationListItem>> GetRecommendationsByChildIdAsync(string childId);
        Task<RecommendationDetailViewModel?> GetRecommendationDetailAsync(Guid recommendationId);
        Task<ServiceResult> GenerateAndSaveForGroupAsync(Guid groupId, string educatorId);
        Task<List<RecommendationListItem>> GetRecommendationsByEducatorIdAsync(string educatorId);
        Task<RecommendationDetailViewModel?> GetGroupRecommendationDetailAsync(Guid recommendationId);
        Task<List<RecommendationListItem>> GetRecommendationsByGroupIdAsync(Guid groupId);
        Task<GptReportOverviewResult?> GenerateReportOverviewAsync(ChildDashboardViewModel data, string viewerRole);


    }
}
