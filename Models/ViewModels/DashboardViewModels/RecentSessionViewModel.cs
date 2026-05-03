namespace LamiePlatform.Models.ViewModels.DashboardViewModels
{
    public class RecentSessionViewModel
    {
        public DateTime PlayedAt { get; set; }
        public string LevelName { get; set; }
        public string IntelligenceName { get; set; }
        public double AspectScore { get; set; }  
        public double TotalTimeSec { get; set; }
    }

}
