namespace LamiePlatform.Models.ViewModels
{
    public class RecommendationViewModel
    {
        public string ChildId { get; set; }
        public List<RecommendationListItem> Recommendations { get; set; } = new();
    }
}
