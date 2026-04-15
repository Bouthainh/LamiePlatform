namespace LamiePlatform.DTOs.GameDTOs
{
    public class AspectResultDto
    {
        public string AspectName { get; set; }
        public float AspectScore { get; set; }
        public string AspectRating { get; set; }
        public int PsychometricPts { get; set; }
        public List<IndicatorResultDto> Indicators { get; set; }
    }
}
