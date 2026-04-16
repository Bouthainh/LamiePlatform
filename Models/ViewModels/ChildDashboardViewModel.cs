namespace LamiePlatform.Models.ViewModels
{
    public class ChildDashboardViewModel
    {
        public string ChildId { get; set; }
        public string ChildName { get; set; }
        public int Age { get; set; }
        public string IconImgPath { get; set; }
        public string EducatorName { get; set; }
        public string EducatorEmail { get; set; }
        public string EducatorId { get; set; }
        public string ParentId { get; set; }
        public List<ChartItemViewModel> Top3Intelligences { get; set; }
        public List<ChartItemViewModel> AllIntelligences { get; set; }
        public List<string> Activities { get; set; }
        public List<string> Reports { get; set; }

        public string challenges { get; set; }
        public string level { get; set; }
    }
}
