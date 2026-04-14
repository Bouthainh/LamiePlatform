using LamiePlatform.DTOs.PlatformDTOs;
using LamiePlatform.Models.ViewModels;
using LamiePlatform.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LamiePlatform.Controllers
{
    public class EducatorController : Controller, IEducatorController
    {
        private readonly IRequestService _requestService;
        private readonly IEducatorService _educatorService;
        private readonly IChildService _childService;
        private readonly IDashboardService _dashboardService;



        public EducatorController(IEducatorService educatorService, IChildService childService, IRequestService requestService, IDashboardService dashboardService)
        {
            _educatorService = educatorService;
            _childService = childService;
            _requestService = requestService;
            _dashboardService = dashboardService;
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
            catch (InvalidOperationException ex)
            {
                await LoadSchoolsAsync();
                TempData["ErrorMessage"] = ex.Message; 
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

        [HttpGet]
        public async Task<IActionResult> GetGradesBySchool(Guid schoolId)
        {
            var grades = await _childService.GetGradesBySchoolIdAsync(schoolId);
            return Json(grades.Select(g => new { id = g.GradeId, name = g.GradeName }));
        }

        [HttpGet]
        public async Task<IActionResult> GetClassesByGrade(Guid gradeId)
        {
            var classes = await _childService.GetClassesByGradeIdAsync(gradeId);
            return Json(classes.Select(c => new { id = c.ClassId, name = c.ClassName }));
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SearchChildren(string searchTerm)
        {
            var educatorId = GetCurrentEducatorId();
            if (string.IsNullOrEmpty(educatorId))
                return RedirectToAction("Login");

            var sentRequests = await _requestService.GetSentRequestsByEducatorAsync(educatorId);
            ViewBag.SentRequests = sentRequests;
            ViewBag.SearchTerm = searchTerm;

            if (string.IsNullOrEmpty(searchTerm))
                return View(new List<LamiePlatform.Models.Child>());

            var results = await _requestService.SearchChildrenAsync(searchTerm, educatorId);
            return View(results);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRequest(string childId)
        {
            var educatorId = GetCurrentEducatorId();
            if (string.IsNullOrEmpty(educatorId))
                return RedirectToAction("Login");

            var result = await _requestService.SendRequestAsync(educatorId, childId);

            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction("SearchChildren");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyStudents()
        {
            var educatorId = GetCurrentEducatorId();
            if (string.IsNullOrEmpty(educatorId))
                return RedirectToAction("Login");

            ViewBag.CurrentEducatorId = educatorId;

            var students = await _requestService.GetEducatorStudentsAsync(educatorId);
            return View(students);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteEducatorAccount()
        {
            var educatorId = GetCurrentEducatorId();
            if (string.IsNullOrEmpty(educatorId))
            {
                return RedirectToAction("Login");
            }
            try
            {
                bool success = await _educatorService.DeleteEducatorAccountAsync(educatorId);
                if (!success)
                {
                    TempData["ErrorMessage"] = "الحساب غير موجود";
                    return RedirectToAction("Login");
                }

                await HttpContext.SignOutAsync();
                TempData["SuccessMessage"] = "تم حذف حسابك بنجاح";
                return RedirectToAction("Login");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database Error in DeleteEducatorAccount: {ex.Message}");
                TempData["ErrorMessage"] = "حدث خطأ أثناء حذف البيانات من قاعدة البيانات.";
                return RedirectToAction("ViewEducatorProfile");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid Operation in DeleteEducatorAccount: {ex.Message}");
                TempData["ErrorMessage"] = "العملية غير صالحة. الرجاء المحاولة مرة أخرى.";
                return RedirectToAction("ViewEducatorProfile");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in DeleteEducatorAccount: {ex.Message}");
                TempData["ErrorMessage"] = "حدث خطأ غير متوقع أثناء عملية الحذف. الرجاء المحاولة لاحقاً.";
                return RedirectToAction("ViewEducatorProfile");
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult ViewStudentDashboard(string childId)
        {
            var educatorId = GetCurrentEducatorId();
            if (string.IsNullOrEmpty(educatorId))
                return RedirectToAction("Login");

            var dashboardData = _dashboardService.GetChildDashboard(childId);

            if (dashboardData == null)
            {
                TempData["ErrorMessage"] = "لا توجد بيانات متاحة لهذا الطالب";
                return RedirectToAction("MyStudents");
            }

            return View(dashboardData);
        }
    }
}
