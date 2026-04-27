using LamiePlatform.Data;
using LamiePlatform.Models.ViewModels;
using LamiePlatform.Models.ViewModels.DashboardViewModels;
using LamiePlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LamiePlatform.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IChatService _chatService;
        private readonly LamieDbContext _db;

        public ReportController(IReportService reportService, IChatService chatService, LamieDbContext db)
        {
            _reportService = reportService;
            _chatService = chatService;
            _db = db;
        }

        private string GetCurrentUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        private string GetViewerRole() =>
            User.FindFirstValue("RelationshipType") == "Educator" ? "Educator" : "Parent";

        [HttpGet]
        [Route("Report/Report")]
        public IActionResult Report(string childId)
        {
            if (string.IsNullOrEmpty(childId))
                return RedirectToAction("Index", "Home");

            var child = _db.Children.FirstOrDefault(c => c.ChildId == childId);
            if (child == null) return NotFound();

            var viewerRole = GetViewerRole();

            var reports = _reportService.GetChildReports(childId, viewerRole);

            if (viewerRole == "Educator")
            {
                var parentId = (from pc in _db.ParentChildren
                                where pc.ChildId == childId
                                select pc.ParentId).FirstOrDefault();
                ViewBag.LinkedParentId = parentId;
            }
            else
            {
                var educatorId = (from ep in _db.EducatorPermissions
                                  where ep.ChildId == childId
                                  select ep.EducatorId).FirstOrDefault();
                ViewBag.LinkedEducatorId = educatorId;
            }

            var vm = new ReportIndexViewModel
            {
                ChildId = childId,
                ChildName = child.ChildName,
                ViewerRole = viewerRole,
                Reports = reports
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(string childId)
        {
            if (string.IsNullOrEmpty(childId))
                return RedirectToAction("Index", "Home");

            var userId = GetCurrentUserId();
            var viewerRole = GetViewerRole();

            try
            {
                await _reportService.GenerateReportAsync(childId, viewerRole, userId);
                TempData["SuccessMessage"] = "تم إنشاء التقرير بنجاح";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"حدث خطأ أثناء إنشاء التقرير: {ex.Message}";
            }

            return RedirectToAction("Report", "Report", new { childId });
        }

        [HttpGet]
        public IActionResult Download(string childId, string fileName)
        {
            if (string.IsNullOrEmpty(childId) || string.IsNullOrEmpty(fileName))
                return BadRequest();

            fileName = Path.GetFileName(fileName);
            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot",
                "reports", childId, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/pdf", fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareToChat(string childId, string fileName,
                                                      string educatorId, string parentId)
        {
            if (string.IsNullOrEmpty(educatorId) || string.IsNullOrEmpty(parentId))
            {
                TempData["ErrorMessage"] = "لا يمكن المشاركة: معلومات المحادثة غير مكتملة";
                return RedirectToAction("Report", "Report", new { childId });
            }

            var userId = GetCurrentUserId();
            var senderType = GetViewerRole();

            var reportUrl = Url.Action("Download", "Report",
                                 new { childId, fileName },
                                 Request.Scheme);

            var messageContent = $" تقرير الطفل\n{reportUrl}";

            try
            {
                var conversation = await _chatService.GetOrCreateConversationAsync(educatorId, parentId);

             
                await _chatService.SendMessageAsync(
                    conversation.ConversationId,
                    userId,
                    senderType,
                    messageContent);

                TempData["SuccessMessage"] = "تمت مشاركة التقرير في المحادثة";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"حدث خطأ أثناء المشاركة: {ex.Message}";
            }

            return RedirectToAction("Chat", "Chat", new { educatorId, parentId });
        }
    }
}