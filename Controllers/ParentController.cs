using BadeePlatform.DTOs;
using BadeePlatform.Models;
using BadeePlatform.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BadeePlatform.Controllers
{
    public class ParentController : Controller
    {
        private readonly IChildService _childService;
        private readonly IParentService _parentService;

        public ParentController(IChildService childService, IParentService parentService)
        {
            _childService = childService;
            _parentService = parentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View(new RegisterParentDTO());

        }


        public IActionResult Login()
        {
            return View(new LoginParentDTO());

        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterParentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto); 
            }

            try
            {
                var result = await _parentService.RegisterParentAsync(dto);

                if (result.Success)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction("login");
                }
                else
                {
                    TempData["Error"] = result.Message;
                    return View(dto);
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "حدث خطأ غير متوقع أثناء عملية التسجيل.";
                return View(dto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginParentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _parentService.LoginParentAsync(dto);

            if (result.Success)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.ParentId), 
            new Claim(ClaimTypes.Name, dto.UsernameOrEmail),
            new Claim(ClaimTypes.Role, "Parent")
        };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, 
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) 
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                TempData["Success"] = result.Message; 
                return RedirectToAction("ParentHomePage"); 
            }
            else
            {
                TempData["Error"] = result.Message; 
                return View(dto);
            }
        }

        public IActionResult AddChild(Child child)
        {
            return View();

        }

        public IActionResult EditChildProfile()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteChildProfile(string childId)
        {

            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(parentId))
            {
                return RedirectToAction("Login");
            }

            try {
                bool success = await _childService.DeleteChildProfileAsync(parentId, childId);

                if (!success)
                {
                    TempData["DeleteChildError"] = "الطفل غير موجود في قائمتك";
                    return RedirectToAction("ManageMultipleChildren");
                }

                TempData["DeleteChildSuccess"] = "تم حذف الطفل برقم الهوية " + childId + " بنجاح";
                return RedirectToAction("ManageMultipleChildren");
            }
            catch
            {
                TempData["DeleteChildError"] = "حدث خطأ غير متوقع أثناء عملية الحذف. الرجاء المحاولة لاحقاً.";
                return RedirectToAction("ManageMultipleChildren");
            }
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
