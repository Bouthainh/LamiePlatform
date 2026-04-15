namespace LamiePlatform.DTOs.GameDTOs
{
    public class IndicatorResultDto
    {
        public string IndicatorName { get; set; }
        public float IndicatorScore { get; set; }
        public string IndicatorRating { get; set; }
        public int PsychometricPts { get; set; }
        public List<AssessmentItemDto> Items { get; set; }
    }
}
