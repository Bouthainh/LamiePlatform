using LamiePlatform.Models;
using LamiePlatform.Models.ViewModels;

namespace LamiePlatform.Services
{
    public interface IDashboardService
    {
        ChildDashboardViewModel GetChildDashboard(string childId);
    }
}
