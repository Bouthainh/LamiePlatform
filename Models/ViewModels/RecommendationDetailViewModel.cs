namespace LamiePlatform.Models.ViewModels
{
    public class RecommendationDetailViewModel
    {
        public Guid RecommendationId { get; set; }
        public string ChildId { get; set; }
        public string ChildName { get; set; }

        public Guid? GroupId { get; set; }
        public string? GroupName { get; set; }

        public string ActivityName { get; set; }
        public string? Category { get; set; }
        public string? ActivityDescription { get; set; }
        public int? Duration { get; set; }
    }
}
