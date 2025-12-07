using BadeePlatform.Models;
using BadeePlatform.Models.ViewModels;

namespace BadeePlatform.Services
{
    public interface IDashboardService
    {
        ChildDashboardViewModel GetChildDashboard(string childId);
    }
}
