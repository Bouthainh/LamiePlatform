namespace LamiePlatform.Models.ViewModels.DashboardViewModels
{
    public class ReportIndexViewModel
    {
        public string ChildId { get; set; }
        public string ChildName { get; set; }
        public string ViewerRole { get; set; }
        public List<ReportFileViewModel> Reports { get; set; } = new();
    }
}
