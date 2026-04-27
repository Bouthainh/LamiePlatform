using LamiePlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LamiePlatform.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChildDashboardEducator(string childId)
        {
            var educatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(educatorId))
                return RedirectToAction("Login", "Educator");

            if (string.IsNullOrEmpty(childId))
            {
                TempData["ErrorMessage"] = "معرف الطالب مفقود";
                return RedirectToAction("MyStudents", "Educator");
            }

            var dashboard = _dashboardService.GetChildDashboard(childId, "Educator");
            if (dashboard == null)
            {
                TempData["ErrorMessage"] = "لا توجد بيانات متاحة لهذا الطالب";
                return RedirectToAction("MyStudents", "Educator");
            }

            return View("ChildDashboard", dashboard);
        }


        [Authorize]
        [HttpGet]
        public IActionResult ChildDashboardParent(string childId)
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(parentId))
                return RedirectToAction("Login", "Parent");

            if (string.IsNullOrEmpty(childId))
            {
                TempData["ErrorMessage"] = "معرف الطفل مفقود";
                return RedirectToAction("ManageMultipleChildren", "Parent");
            }

            var dashboard = _dashboardService.GetChildDashboard(childId, "Parent");
            if (dashboard == null)
            {
                TempData["ErrorMessage"] = "لا توجد بيانات متاحة لهذا الطفل";
                return RedirectToAction("ManageMultipleChildren", "Parent");
            }

            return View("ChildDashboard", dashboard);
        }
    }
}
