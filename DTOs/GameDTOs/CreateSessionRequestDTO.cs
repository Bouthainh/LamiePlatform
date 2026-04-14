namespace LamiePlatform.DTOs.GameDTOs
{
    public class CreateSessionRequest
    {
        public string ChildId { get; set; }
        public Guid LevelId { get; set; }
        public float TotalTime { get; set; }
    }
}
