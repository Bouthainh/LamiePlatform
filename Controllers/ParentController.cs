using BadeePlatform.Data;
using BadeePlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BadeePlatform.Controllers
{
    public class ParentController : Controller
    {
        private readonly BadeedbContext _db;

        public ParentController(BadeedbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        // TODAY
        public IActionResult Register()
        {
            return View();

        }

        // TODAY
        public IActionResult Login()
        {
            return View();

        }

        // TODAY

        public IActionResult AddChild()
        {
            return View();

        }

        // TODAY
        public IActionResult GenerateChildCode()
        {
            return View();

        }

        // TODAY
        public IActionResult EditChildProfile()
        {
            return View();
        }

        // TODAY
        [HttpPost]
        public async Task<IActionResult> DeleteChildProfile(String childId)
        {

            var parentId = HttpContext.Session.GetString("ParentId");

            if (string.IsNullOrEmpty(parentId))
            {
                return RedirectToAction("Login");
            }

            var child = await _db.ParentChildren.FirstOrDefaultAsync(pc => pc.ParentId == parentId && pc.ChildId == childId);
            
            if (child == null)
            {
                TempData["Error"] = "الطفل غير موجود في قائمتك";
                return RedirectToAction("ManageMultipleChildren");
            }

            _db.ParentChildren.Remove(child);
            await _db.SaveChangesAsync();
            TempData["DeleteChildSuccess"] = "تم حذف الطفل برقم الهوية " + childId + " بنجاح";
            return RedirectToAction("ManageMultipleChildren");
        }

        public IActionResult ViewChildProfile()
        {
            return View();

        }

        public IActionResult ViewChildDashboard()
        {
            return View();

        }

    }
}
