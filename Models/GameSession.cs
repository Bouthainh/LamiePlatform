using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class GameSession
{
    public Guid LevelId { get; set; }

    public string ChildId { get; set; } = null!;

    public Guid GameSessionId { get; set; }

    public double? TotalTime { get; set; }

    public DateTime? PlayedAt { get; set; }

    public virtual ICollection<AspectResult> AspectResults { get; set; } = new List<AspectResult>();

    public virtual Child Child { get; set; } = null!;

    public virtual GameLevel Level { get; set; } = null!;
}
