using Microsoft.AspNetCore.Mvc;
using LamiePlatform.DTOs.PlatformDTOs;

namespace LamiePlatform.Controllers
{
    public interface IEducatorController 
    {
            IActionResult Index();
            Task<IActionResult> Register();
            Task<IActionResult> Register(RegisterEducatorDTO dto);
            IActionResult Login();
            Task<IActionResult> Login(LoginEducatorDTO dto);
            Task<IActionResult> Logout();
            Task<IActionResult> GetGradesBySchool(Guid schoolId);
            Task<IActionResult> GetClassesByGrade(Guid gradeId);


    }
}
