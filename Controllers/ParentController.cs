using BadeePlatform.DTOs;
using BadeePlatform.Models;
using BadeePlatform.Models.ViewModels;
using BadeePlatform.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BadeePlatform.Controllers
{
    public class ParentController : Controller, IParentController
    {
        private readonly IChildService _childService;
        private readonly IParentService _parentService;
        private readonly IDashboardService _dashboardService;

        public ParentController(IChildService childService, IParentService parentService, IDashboardService dashboardService)
        {
            _childService = childService;
            _parentService = parentService;
            _dashboardService = dashboardService;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("ParentHomePage");
            }

            return View(new RegisterParentDTO());
        }

        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("ParentHomePage");
            }

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
                    TempData["RegisterSuccessMessage"] = result.Message;
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewData["ErrorMessage"] = result.Message;
                    return View(dto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register Error: {ex.Message}");
                ViewData["ErrorMessage"] = "حدث خطأ غير متوقع أثناء عملية التسجيل.";
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
                string relationshipType = result.Data ?? "Parent";
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.ParentId),
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
                return RedirectToAction("ParentHomePage");
            }
            else
            {
                ViewData["ErrorMessage"] = result.Message;
                return View(dto);
            }
        }

        [Authorize]
        public async Task<IActionResult> AddChild()
        {
            await LoadCitiesForDropdown();
            return View(new AddChildDTO());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddChild(AddChildDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesForDropdown();
                return View(dto);
            }

            if (dto.SchoolId == null || dto.GradeId == null || dto.ClassId == null)
            {
                ViewData["ErrorMessage"] = "الرجاء اختيار المدرسة والمرحلة والفصل.";
                await LoadCitiesForDropdown();
                return View(dto);
            }

            var parentId = GetCurrentParentId();
            if (string.IsNullOrEmpty(parentId))
            {
                return RedirectToAction("Login");
            }

            string relationshipType = User.FindFirstValue("RelationshipType");
            var result = await _childService.AddChildAsync(parentId, dto, relationshipType);

            if (result.Success)
            {
                TempData["ChildSuccessMessage"] = result.Message;

                if (!string.IsNullOrEmpty(result.Data))
                {
                    TempData["LoginCode"] = result.Data;
                }

                return RedirectToAction("ManageMultipleChildren");
            }
            else
            {
                ViewData["ErrorMessage"] = result.Message;
                await LoadCitiesForDropdown();
                return View(dto);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteChildProfile(string childId)
        {
            var parentId = GetCurrentParentId();

            if (string.IsNullOrEmpty(parentId))
            {
                return RedirectToAction("Login");
            }

            try
            {
                bool success = await _childService.DeleteChildProfileAsync(parentId, childId);

                if (!success)
                {
                    TempData["ErrorMessage"] = "الطفل غير موجود في قائمتك";
                    return RedirectToAction("ManageMultipleChildren");
                }

                TempData["ChildSuccessMessage"] = "تم حذف الطفل صاحب رقم الهوية " + childId + " بنجاح";
                return RedirectToAction("ManageMultipleChildren");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database Error in DeleteChild: {ex.Message}");
                TempData["ErrorMessage"] = "حدث خطأ أثناء حذف البيانات من قاعدة البيانات.";
                return RedirectToAction("ManageMultipleChildren");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid Operation in DeleteChild: {ex.Message}");
                TempData["ErrorMessage"] = "العملية غير صالحة. الرجاء المحاولة مرة أخرى.";
                return RedirectToAction("ManageMultipleChildren");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in DeleteChild: {ex.Message}");
                TempData["ErrorMessage"] = "حدث خطأ غير متوقع أثناء عملية الحذف. الرجاء المحاولة لاحقاً.";
                return RedirectToAction("ManageMultipleChildren");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSchoolsByCity(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                return Json(new List<School>());
            }

            var schools = await _childService.GetSchoolsByCityAsync(city);
            return Json(schools.Select(s => new
            {
                id = s.SchoolId,
                name = s.SchoolName
            }));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetGradesBySchool(string schoolId)
        {
            if (string.IsNullOrEmpty(schoolId) || !Guid.TryParse(schoolId, out Guid parsedSchoolId))
            {
                return Json(new List<Grade>());
            }

            var grades = await _childService.GetGradesBySchoolIdAsync(parsedSchoolId);
            return Json(grades.Select(g => new
            {
                id = g.GradeId,
                name = g.GradeName
            }));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetClassesByGrade(string gradeId)
        {
            if (string.IsNullOrEmpty(gradeId) || !Guid.TryParse(gradeId, out Guid parsedGradeId))
            {
                return Json(new List<Class>());
            }

            var classes = await _childService.GetClassesByGradeIdAsync(parsedGradeId);
            return Json(classes.Select(c => new
            {
                id = c.ClassId,
                name = c.ClassName,
                educator = c.Educator?.EducatorName
            }));
        }

        [Authorize]
        [Route("Parent/ViewChildProfile/{childId}")]
        public async Task<IActionResult> ViewChildProfile(string childId)
        {
            if (string.IsNullOrEmpty(childId))
            {
                TempData["ErrorMessage"] = "رقم هوية الطفل غير صحيح.";
                return RedirectToAction("ManageMultipleChildren");
            }

            var parentId = GetCurrentParentId();

            if (string.IsNullOrEmpty(parentId))
            {
                return RedirectToAction("Login");
            }

            var childProfile = await _childService.GetChildProfileByIdAsync(parentId, childId);

            if (childProfile == null)
            {
                TempData["ErrorMessage"] = "الطفل غير موجود أو ليس لديك صلاحية لعرضه.";
                return RedirectToAction("ManageMultipleChildren");
            }

            return View(childProfile);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GrantEducatorAccess(string childId)
        {
            var parentId = GetCurrentParentId();

            if (string.IsNullOrEmpty(parentId) || string.IsNullOrEmpty(childId))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var result = await _childService.GrantEducatorAccessAsync(parentId, childId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }

                return RedirectToAction("ViewChildProfile", new { childId });
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء منح الصلاحية في قاعدة البيانات.";
                return RedirectToAction("ViewChildProfile", new { childId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = "العملية غير صالحة. الرجاء المحاولة مرة أخرى.";
                return RedirectToAction("ViewChildProfile", new { childId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ غير متوقع. الرجاء المحاولة لاحقاً.";
                return RedirectToAction("ViewChildProfile", new { childId });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RevokeEducatorAccess(string childId)
        {
            var parentId = GetCurrentParentId();

            if (string.IsNullOrEmpty(parentId) || string.IsNullOrEmpty(childId))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var result = await _childService.RevokeEducatorAccessAsync(parentId, childId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }

                return RedirectToAction("ViewChildProfile", new { childId });
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء إلغاء الصلاحية في قاعدة البيانات.";
                return RedirectToAction("ViewChildProfile", new { childId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = "العملية غير صالحة. الرجاء المحاولة مرة أخرى.";
                return RedirectToAction("ViewChildProfile", new { childId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ غير متوقع. الرجاء المحاولة لاحقاً.";
                return RedirectToAction("ViewChildProfile", new { childId });
            }
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditChildProfile(string childId)
        {
            if (string.IsNullOrEmpty(childId))
            {
                TempData["ErrorMessage"] = "رقم هوية الطفل غير صحيح";
                return RedirectToAction("ManageMultipleChildren");
            }

            var parentId = GetCurrentParentId();

            if (string.IsNullOrEmpty(parentId))
            {
                return RedirectToAction("Login");
            }

            var childDto = await _childService.GetChildForEditAsync(parentId, childId);

            if (childDto == null)
            {
                TempData["ErrorMessage"] = "الطفل غير موجود أو ليس لديك صلاحية لتعديله";
                return RedirectToAction("ManageMultipleChildren");
            }

            await LoadCitiesForDropdown();
            ViewBag.ChildId = childId;

            return View(childDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditChildProfile(string childId, EditChildDTO dto)
        {
            if (string.IsNullOrEmpty(childId))
            {
                TempData["ErrorMessage"] = "رقم هوية الطفل غير صحيح";
                return RedirectToAction("ManageMultipleChildren");
            }

            if (!ModelState.IsValid)
            {
                await LoadCitiesForDropdown();
                ViewBag.ChildId = childId;
                return View(dto);
            }

            var parentId = GetCurrentParentId();

            if (string.IsNullOrEmpty(parentId))
            {
                return RedirectToAction("Login");
            }

            var result = await _childService.EditChildProfileAsync(parentId, childId, dto);

            if (result.Success)
            {
                TempData["ChildSuccessMessage"] = result.Message;
                return RedirectToAction("ViewChildProfile", new { childId });
            }
            else
            {
                ViewData["ErrorMessage"] = result.Message;
                await LoadCitiesForDropdown();
                ViewBag.ChildId = childId;
                return View(dto);
            }
        }


        private string GetCurrentParentId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task LoadCitiesForDropdown()
        {
            var cities = await _childService.GetAllCitiesAsync();
            ViewBag.Cities = cities;
        }


        public async Task<IActionResult> ParentHomePage()
        {
            var parentId = GetCurrentParentId();
            if (string.IsNullOrEmpty(parentId))
            {
                return RedirectToAction("Login");
            }

            var children = await _childService.GetAllChildrenByParentIdAsync(parentId);

            return View(children);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ViewChildDashboard(string childId)
        {
            var dashboardData = _dashboardService.GetChildDashboard(childId);

            if (dashboardData == null)
            {
                TempData["ErrorMessage"] = "لا توجد بيانات متاحة للعرض في لوحة التحكم لهذا الطفل";
                return RedirectToAction("ManageMultipleChildren");
            }

            return View("ViewChildDashboard", dashboardData);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ManageMultipleChildren()
        {
            var parentId = GetCurrentParentId();
            if (string.IsNullOrEmpty(parentId))
            {
                return RedirectToAction("Login");
            }

            var children = await _childService.GetAllChildrenByParentIdAsync(parentId);

            return View(children);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewProfile()
        {
            var parentId = GetCurrentParentId();

            if (string.IsNullOrEmpty(parentId))
                return RedirectToAction("Login");

            var profile = await _parentService.GetParentProfileAsync(parentId);

            if (profile == null)
                return NotFound();

            return View(profile);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ParentProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View("ViewProfile", model);

            try
            {
                var success = await _parentService.UpdateParentProfileAsync(model);

                if (success)
                {
                    TempData["ProfileEditedSuccessMessage"] = "تم حفظ التعديلات بنجاح";
                    return RedirectToAction("ViewProfile");
                }

                TempData["ErrorMessage"] = "حدث خطأ أثناء حفظ البيانات";
                return View("ViewProfile", model);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {

                TempData["ErrorMessage"] = "تعذر حفظ البيانات. قد يكون هناك بيانات مكررة أو مشكلة في قاعدة البيانات.";
                return View("ViewProfile", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ غير متوقع. يرجى المحاولة مرة أخرى لاحقًا.";
                return View("ViewProfile", model);
            }

        }
    }
}