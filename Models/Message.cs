using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class Message
{
    public Guid MessageId { get; set; }

    public Guid ConversationId { get; set; }

    public string SenderType { get; set; } = null!;

    public string SenderId { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;
}
