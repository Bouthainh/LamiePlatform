namespace LamiePlatform.DTOs
{
    public class GptRecommendationResult
    {
        public string ActivityName { get; set; }
        public string? Category { get; set; }
        public string? ActivityDescription { get; set; }
        public string Duration { get; set; }
    }
}
