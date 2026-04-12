using BadeePlatform.Services;
using LamiePlatform.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BadeePlatform.Controllers

{
    public class RecommendationController : Controller
    {

        private readonly IRecommendationService _recommendationService;

        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        // PARENT Actions

        public async Task<IActionResult> ActivityList(string childId)
        {
            var recommendations = await _recommendationService.GetRecommendationsByChildIdAsync(childId);

            var vm = new RecommendationViewModel
            {
                ChildId = childId,
                Recommendations = recommendations
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateForChild(string childId)
        {
            var result = await _recommendationService.GenerateAndSaveAsync(childId);

            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(ActivityList), new { childId });
        }

        public async Task<IActionResult> ActivityDetail(Guid id)
        {
            var vm = await _recommendationService.GetRecommendationDetailAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // EDUCATOR Actions 

        public async Task<IActionResult> EdActivityList(string educatorId, Guid? groupId)
        {
            var recommendations = groupId.HasValue
                ? await _recommendationService.GetRecommendationsByGroupIdAsync(groupId.Value)
                : new List<RecommendationListItem>();

            var vm = new EducatorRecommendationViewModel
            {
                EducatorId = educatorId,
                GroupId = groupId,
                Recommendations = recommendations
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateForGroup(Guid groupId, string educatorId)
        {
            var result = await _recommendationService.GenerateAndSaveForGroupAsync(groupId, educatorId);

            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(EdActivityList), new { educatorId, groupId });
        }

        public async Task<IActionResult> EdActivityDetail(Guid id)
        {
            var vm = await _recommendationService.GetGroupRecommendationDetailAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }
    }
}
