using BadeePlatform.Data;
using BadeePlatform.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BadeePlatform.Controllers;

public class GroupingController : Controller
{
    private readonly IGroupingService _groupingService;
    private readonly BadeedbContext _context;

    public GroupingController(IGroupingService groupingService, BadeedbContext context)
    {
        _groupingService = groupingService;
        _context = context;
    }

    //  يجيب فصل المعلم 
    public async Task<IActionResult> Index()
    {
        var educatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var educatorClass = await _context.Classes
            .FirstOrDefaultAsync(c => c.EducatorId == educatorId);

        if (educatorClass == null)
        {
            TempData["Error"] = "لا يوجد فصل مرتبط بحسابك";
            return RedirectToAction("EducatorHomePage", "Educator");
        }

        return RedirectToAction("MyGroups", new { classId = educatorClass.ClassId });
    }

    public async Task<IActionResult> MyGroups(Guid classId)
    {
        var educatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.EducatorId = educatorId;
        ViewBag.ClassId = classId;

        var alreadyGrouped = await _groupingService.ClassAlreadyGroupedAsync(classId);
        if (alreadyGrouped)
        {
            var groups = await _groupingService.GetGroupsByClassAsync(classId);
            ViewBag.ClassId = classId;
            return View("~/Views/Educator/Grouping/MyGroups.cshtml", groups);
        }
        ViewBag.ClassId = classId;
        ViewBag.NotGroupedYet = true;
        return View("~/Views/Educator/Grouping/MyGroups.cshtml", new List<BadeePlatform.Models.ChildGroup>());
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroups(Guid classId)
    {
        try
        {
            await _groupingService.CreateGroupsForClassAsync(classId);
            TempData["Success"] = "Groups created successfully!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("MyGroups", new { classId });
    }
}