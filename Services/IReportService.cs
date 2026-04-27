using LamiePlatform.Models.ViewModels.DashboardViewModels;
namespace LamiePlatform.Services
{
    public interface IReportService
    {
        Task<string> GenerateReportAsync(string childId, string viewerRole, string generatedBy);
        List<ReportFileViewModel> GetChildReports(string childId, string viewerRole);
    }
}
