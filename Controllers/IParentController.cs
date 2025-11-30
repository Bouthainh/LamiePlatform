using BadeePlatform.DTOs;
using BadeePlatform.Models;
using Microsoft.AspNetCore.Mvc;

namespace BadeePlatform.Controllers
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

        public IActionResult ViewChildDashboard(string childId);
        public IActionResult ViewParentHomePage();

        public Task<IActionResult> GetSchoolsByCity(string city);

        public Task<IActionResult> GetGradesBySchool(string schoolId);

        public Task<IActionResult> GetClassesByGrade(string gradeId);
        public Task<IActionResult> GrantEducatorAccess(string childId);
        public Task<IActionResult> RevokeEducatorAccess(string childId);





    }
}
