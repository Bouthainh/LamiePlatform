namespace LamiePlatform.Models.ViewModels.DashboardViewModels
{
    public class TopIntelligenceSummaryViewModel
    {
        public string IntelligenceName { get; set; }
        public int Score { get; set; }
        public string BestAspectName { get; set; }
        public double BestAspectScore { get; set; }
        public string? WeakAspectName { get; set; }
        public double WeakAspectScore { get; set; }
    }
}
