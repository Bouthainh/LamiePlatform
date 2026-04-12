using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class Request
{
    public Guid RequestId { get; set; }

    public string? EducatorId { get; set; }

    public string? ChildId { get; set; }

    public string? ParentId { get; set; }

    public string? RequestStatus { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual Child? Child { get; set; }

    public virtual Educator? Educator { get; set; }

    public virtual Parent? Parent { get; set; }
}
