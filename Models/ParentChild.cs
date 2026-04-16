using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class ParentChild
{
    public string ParentId { get; set; } = null!;

    public string ChildId { get; set; } = null!;

    public string? RelationshipType { get; set; }

    public virtual Child Child { get; set; } = null!;

    public virtual Parent Parent { get; set; } = null!;
}
