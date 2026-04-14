using LamiePlatform.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LamiePlatform.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        private string GetCurrentUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        private string GetCurrentUserType() =>
            User.FindFirstValue("RelationshipType") == "Educator" ? "Educator" : "Parent";

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var userType = GetCurrentUserType();
            var conversations = await _chatService.GetConversationsAsync(userId, userType);
            ViewBag.UserType = userType;
            ViewBag.UserId = userId;
            return View(conversations);
        }

        [HttpGet]
        public async Task<IActionResult> Chat(string educatorId, string parentId)
        {
            if (string.IsNullOrEmpty(educatorId) || string.IsNullOrEmpty(parentId))
            {
                TempData["ErrorMessage"] = "معلومات المحادثة غير مكتملة";
                return RedirectToAction("Index");
            }

            var userId = GetCurrentUserId();
            var userType = GetCurrentUserType();

            var conversation = await _chatService.GetOrCreateConversationAsync(educatorId, parentId);
            var messages = await _chatService.GetMessagesAsync(conversation.ConversationId);

            // Mark messages as read when opening chat
            await _chatService.MarkMessagesAsReadAsync(conversation.ConversationId, userType);

            ViewBag.ConversationId = conversation.ConversationId;
            ViewBag.UserType = userType;
            ViewBag.UserId = userId;
            ViewBag.EducatorId = educatorId;
            ViewBag.ParentId = parentId;

            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Guid conversationId, string content, string educatorId, string parentId)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Chat", new { educatorId, parentId });

            var userId = GetCurrentUserId();
            var userType = GetCurrentUserType();

            await _chatService.SendMessageAsync(conversationId, userId, userType, content);

            return RedirectToAction("Chat", new { educatorId, parentId });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            var userType = GetCurrentUserType();
            var count = await _chatService.GetUnreadCountAsync(userId, userType);
            return Json(new { count });
        }
    }
}
