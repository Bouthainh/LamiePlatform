using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class EducatorPermission
{
    public Guid RequestId { get; set; }

    public string EducatorId { get; set; } = null!;

    public string? ChildId { get; set; }

    public string? ParentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Child? Child { get; set; }

    public virtual Educator Educator { get; set; } = null!;

    public virtual Parent? Parent { get; set; }
}
