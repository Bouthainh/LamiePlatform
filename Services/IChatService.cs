using  LamiePlatform.Models;

namespace LamiePlatform.Services
{
    public interface IChatService
    {

        Task<Conversation> GetOrCreateConversationAsync(string educatorId, string parentId);
        Task<List<Message>> GetMessagesAsync(Guid conversationId);
        Task<Message> SendMessageAsync(Guid conversationId, string senderId, string senderType, string content);
        Task MarkMessagesAsReadAsync(Guid conversationId, string senderType);
        Task<int> GetUnreadCountAsync(string userId, string userType);
        Task<List<Conversation>> GetConversationsAsync(string userId, string userType);
    }
}
