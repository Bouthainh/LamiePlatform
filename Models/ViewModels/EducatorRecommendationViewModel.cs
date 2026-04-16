namespace LamiePlatform.Models.ViewModels
{
    public class EducatorRecommendationViewModel
    {
        public string EducatorId { get; set; } = "";
        public Guid? GroupId { get; set; }
        public List<RecommendationListItem> Recommendations { get; set; } = new();
    }
}
