namespace LamiePlatform.Models.ViewModels.DashboardViewModels
{
    public class AspectDetailViewModel
    {
        public string AspectName { get; set; }
        public double AspectScore { get; set; }
        public string AspectRating { get; set; }
        public List<IndicatorDetailViewModel> Indicators { get; set; } = new();
    }
}
