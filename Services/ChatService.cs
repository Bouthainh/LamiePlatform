using LamiePlatform.Data;
using LamiePlatform.Models;
using Microsoft.EntityFrameworkCore;


namespace LamiePlatform.Services
{
    public class ChatService : IChatService
    {
        private readonly LamieDbContext _db;

        public ChatService(LamieDbContext db)
        {
            _db = db;
        }

        public async Task<Conversation> GetOrCreateConversationAsync(string educatorId, string parentId)
        {
            var conversation = await _db.Conversations
                .FirstOrDefaultAsync(c => c.EducatorId == educatorId && c.ParentId == parentId);

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    EducatorId = educatorId,
                    ParentId = parentId,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Conversations.Add(conversation);
                await _db.SaveChangesAsync();
            }

            return conversation;
        }

        public async Task<List<Message>> GetMessagesAsync(Guid conversationId)
        {
            return await _db.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<Message> SendMessageAsync(Guid conversationId, string senderId, string senderType, string content)
        {
            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                SenderType = senderType,
                Content = content,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
            return message;
        }

        public async Task MarkMessagesAsReadAsync(Guid conversationId, string readerType)
        {
            var otherType = readerType == "Educator" ? "Parent" : "Educator";

            var unreadMessages = await _db.Messages
                .Where(m => m.ConversationId == conversationId
                         && m.SenderType == otherType
                         && !m.IsRead)
                .ToListAsync();

            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
            }

            await _db.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId, string userType)
        {
            var otherType = userType == "Educator" ? "Parent" : "Educator";

            return await _db.Messages
                .Where(m => !m.IsRead
                         && m.SenderType == otherType
                         && m.Conversation.EducatorId == (userType == "Educator" ? userId : m.Conversation.EducatorId)
                         && m.Conversation.ParentId == (userType == "Parent" ? userId : m.Conversation.ParentId))
                .CountAsync();
        }

        public async Task<List<Conversation>> GetConversationsAsync(string userId, string userType)
        {
            return await _db.Conversations
                .Include(c => c.Educator)
                .Include(c => c.Parent)
                .Include(c => c.Messages)
                .Where(c => userType == "Educator"
                    ? c.EducatorId == userId
                    : c.ParentId == userId)
                .OrderByDescending(c => c.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => m.SentAt)
                    .FirstOrDefault())
                .ToListAsync();
        }

    }
}
