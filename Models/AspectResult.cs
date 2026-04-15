using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class AspectResult
{
    public int AspectResultId { get; set; }

    public Guid GameSessionId { get; set; }

    public Guid IntelligenceId { get; set; }

    public string AspectName { get; set; } = null!;

    public double AspectScore { get; set; }

    public string AspectRating { get; set; } = null!;

    public DateTime? RecordedAt { get; set; }

    public virtual GameSession GameSession { get; set; } = null!;

    public virtual ICollection<IndicatorResult> IndicatorResults { get; set; } = new List<IndicatorResult>();

    public virtual IntelligenceType Intelligence { get; set; } = null!;
}
