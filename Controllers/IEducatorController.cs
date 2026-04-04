using Microsoft.AspNetCore.Mvc;
using BadeePlatform.DTOs;

namespace BadeePlatform.Controllers
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
