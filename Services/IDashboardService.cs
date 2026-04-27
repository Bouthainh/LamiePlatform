using LamiePlatform.Models;
using LamiePlatform.Models.ViewModels.DashboardViewModels;

namespace LamiePlatform.Services
{
    public interface IDashboardService
    {
        ChildDashboardViewModel GetChildDashboard(string childId, string viewerRole);
    }
}
