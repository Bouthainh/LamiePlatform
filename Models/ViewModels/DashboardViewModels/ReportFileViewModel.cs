namespace LamiePlatform.Models.ViewModels.DashboardViewModels
{
    public class ReportFileViewModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }      
        public DateTime GeneratedAt { get; set; }
        public string GeneratedBy { get; set; }
        public string Role { get; set; }
        public string ChildId { get; set; }
    }
}
