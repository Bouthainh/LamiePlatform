using BadeePlatform.DTOs;
using BadeePlatform.Models.ViewModels;
using BadeePlatform.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BadeePlatform.Controllers
{
    public class EducatorController : Controller, IEducatorController
    {
        private readonly IEducatorService _educatorService; 

        public EducatorController(IEducatorService educatorService)
        {
            _educatorService = educatorService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("EducatorHomePage");
            }
            await LoadSchoolsAsync();
            return View(new RegisterEducatorDTO());
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterEducatorDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadSchoolsAsync();
                return View(dto);
            }

            try
            {
                var result = await _educatorService.RegisterEducatorAsync(dto);

                if (result.Success)
                {
                    TempData["RegisterSuccessMessage"] = result.Message;
                    return RedirectToAction("Login");
                }
                else
                {
                    await LoadSchoolsAsync();
                    ViewData["ErrorMessage"] = result.Message;
                    return View(dto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register Error: {ex.Message}");
                await LoadSchoolsAsync();
                ViewData["ErrorMessage"] = "حدث خطأ غير متوقع أثناء عملية التسجيل.";
                return View(dto);
            }
        }

        private async Task LoadSchoolsAsync()
        {
            ViewBag.Schools = new SelectList(
                await _educatorService.GetAllSchoolsAsync(), "SchoolId", "SchoolName");
        }

        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("EducatorHomePage");
            }

            return View(new LoginEducatorDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginEducatorDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _educatorService.LoginEducatorAsync(dto);

            if (result.Success)
            {
                string relationshipType = result.Data ?? "Educator";
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.UserId),
                    new Claim(ClaimTypes.Name, dto.UsernameOrEmail),
                    new Claim("RelationshipType", relationshipType)
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

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("EducatorHomePage");
            }
            else
            {
                ViewData["ErrorMessage"] = result.Message;
                return View(dto);
            }
        }

        public IActionResult EducatorHomePage()
        {
            var EducatorId = GetCurrentEducatorId();
            if (string.IsNullOrEmpty(EducatorId))
            {
                return RedirectToAction("Login");
            }

            return View();
        }
        private string GetCurrentEducatorId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewEducatorProfile()
        {
            var educatorId = GetCurrentEducatorId();
            if (string.IsNullOrEmpty(educatorId))
                return RedirectToAction("Login");

            var profile = await _educatorService.GetEducatorProfileAsync(educatorId);
            if (profile == null)
                return NotFound();

            await LoadSchoolsAsync();
            return View(profile);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EducatorProfileViewModel model)
        {

            if (!ModelState.IsValid)
            {
                await LoadSchoolsAsync(); 
                return View("ViewEducatorProfile", model);
            }
            try
            {
                var success = await _educatorService.UpdateEducatorProfileAsync(model);
                if (success)
                {
                    TempData["ProfileEditedSuccessMessage"] = "تم حفظ التعديلات بنجاح";
                    return RedirectToAction("ViewEducatorProfile");
                }
                await LoadSchoolsAsync(); 
                TempData["ErrorMessage"] = "حدث خطأ أثناء حفظ البيانات";
                return View("ViewEducatorProfile", model);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                await LoadSchoolsAsync(); 
                TempData["ErrorMessage"] = "تعذر حفظ البيانات. قد يكون هناك بيانات مكررة أو مشكلة في قاعدة البيانات.";
                return View("ViewEducatorProfile", model);
            }
            catch (Exception ex)
            {
                await LoadSchoolsAsync(); 
                TempData["ErrorMessage"] = "حدث خطأ غير متوقع. يرجى المحاولة مرة أخرى لاحقًا.";
                return View("ViewEducatorProfile", model);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
