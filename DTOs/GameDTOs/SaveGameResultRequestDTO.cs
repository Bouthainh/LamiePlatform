namespace LamiePlatform.DTOs.GameDTOs
{
    public class SaveGameResultRequest
    {
        public string ChildId { get; set; }
        public string LevelId { get; set; }
        public float TotalTime { get; set; }
        public AspectResultDto AspectResult { get; set; }
    }
}
