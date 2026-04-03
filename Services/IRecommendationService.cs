using BadeePlatform.Services;
using LamiePlatform.Models.ViewModels;


namespace BadeePlatform.Services
{
    public interface IRecommendationService
    {
        Task<ServiceResult> GenerateAndSaveAsync(string childId, string? educatorId = null);
        Task<List<RecommendationListItem>> GetRecommendationsByChildIdAsync(string childId);
        Task<RecommendationDetailViewModel?> GetRecommendationDetailAsync(Guid recommendationId);
        Task<ServiceResult> GenerateAndSaveForGroupAsync(Guid groupId, string educatorId);
        Task<List<RecommendationListItem>> GetRecommendationsByEducatorIdAsync(string educatorId);
        Task<RecommendationDetailViewModel?> GetGroupRecommendationDetailAsync(Guid recommendationId);

    }
}
