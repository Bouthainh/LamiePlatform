using LamiePlatform.DTOs.PlatformDTOs;
using LamiePlatform.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LamiePlatform.Controllers
{
    public interface IParentController
    {
        public IActionResult Index();
        public IActionResult Register();
        public IActionResult Login();
        public Task<IActionResult> Register(RegisterParentDTO dto);
        public Task<IActionResult> Login(LoginParentDTO dto);
        public Task<IActionResult> AddChild();
        public Task<IActionResult> AddChild(AddChildDTO dto);
        public Task<IActionResult> DeleteChildProfile(string childId);
        public Task<IActionResult> ViewChildProfile(string childId);
        public Task<IActionResult> EditChildProfile(string childId);
        public Task<IActionResult> EditChildProfile(string childId, EditChildDTO dto);
        public Task<IActionResult> ParentHomePage();
        public Task<IActionResult> ViewProfile();
        public Task<IActionResult> EditProfile(ParentProfileViewModel model);
        public Task<IActionResult> GetSchoolsByCity(string city);
        public Task<IActionResult> GetGradesBySchool(string schoolId);
        public Task<IActionResult> GetClassesByGrade(string gradeId);
        public Task<IActionResult> GrantEducatorAccess(string childId);
        public Task<IActionResult> RevokeEducatorAccess(string childId);
        public Task<IActionResult> ManageMultipleChildren();
        public Task<IActionResult> Logout();

    }
}
