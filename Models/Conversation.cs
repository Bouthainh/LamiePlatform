using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class Conversation
{
    public Guid ConversationId { get; set; }

    public string EducatorId { get; set; } = null!;

    public string ParentId { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Educator Educator { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Parent Parent { get; set; } = null!;
}
