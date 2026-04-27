namespace LamiePlatform.Models.ViewModels.DashboardViewModels
{
    public class IntelligenceDetailViewModel
    {
        public Guid IntelligenceId { get; set; }
        public string IntelligenceName { get; set; }
        public int ProficiencyScore { get; set; }
        public string IntelligenceLevel { get; set; }
        public int TotalLevels { get; set; }       
        public int CompletedLevels { get; set; }
        public bool IsUnderMeasurement { get; set; }  
        public List<LevelStatusViewModel> Levels { get; set; } = new();
        public List<AspectDetailViewModel> Aspects { get; set; } = new();
    }
}
