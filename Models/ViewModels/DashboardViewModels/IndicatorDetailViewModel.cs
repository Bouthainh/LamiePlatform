namespace LamiePlatform.Models.ViewModels.DashboardViewModels
{
    public class IndicatorDetailViewModel
    {
        public string IndicatorName { get; set; }
        public double IndicatorScore { get; set; }
        public string IndicatorRating { get; set; }
        public int PsychometricPts { get; set; }
        public List<AssessmentItemViewModel> AssessmentItems { get; set; } = new();

    }
}
