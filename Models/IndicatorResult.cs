using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class IndicatorResult
{
    public int IndicatorResultId { get; set; }

    public int AspectResultId { get; set; }

    public string IndicatorName { get; set; } = null!;

    public double IndicatorScore { get; set; }

    public string IndicatorRating { get; set; } = null!;

    public int PsychometricPts { get; set; }

    public DateTime? RecordedAt { get; set; }

    public virtual AspectResult AspectResult { get; set; } = null!;

    public virtual ICollection<AssessmentItem> AssessmentItems { get; set; } = new List<AssessmentItem>();
}
