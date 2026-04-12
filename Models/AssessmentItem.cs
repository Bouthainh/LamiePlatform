using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class AssessmentItem
{
    public int ItemId { get; set; }

    public int IndicatorResultId { get; set; }

    public int ItemIndex { get; set; }

    public double Accuracy { get; set; }

    public double? SpeedScore { get; set; }

    public double ErrorRate { get; set; }

    public double FinalScore { get; set; }

    public string Rating { get; set; } = null!;

    public int PsychometricPts { get; set; }

    public DateTime? RecordedAt { get; set; }

    public virtual IndicatorResult IndicatorResult { get; set; } = null!;
}
