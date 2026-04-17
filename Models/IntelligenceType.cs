using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class IntelligenceType
{
    public Guid IntelligenceId { get; set; }

    public string? IntelligenceName { get; set; }

    public string? IntelligenceTypeDescription { get; set; }

    public string? IconImgPath { get; set; }

    public virtual ICollection<AspectResult> AspectResults { get; set; } = new List<AspectResult>();

    public virtual ICollection<ChildIntelligence> ChildIntelligences { get; set; } = new List<ChildIntelligence>();

    public virtual ICollection<GameLevel> GameLevels { get; set; } = new List<GameLevel>();
}