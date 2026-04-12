using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class ChildIntelligence
{
    public string ChildId { get; set; } = null!;

    public Guid IntelligenceId { get; set; }

    public decimal? ProficiencyScore { get; set; }

    public string? IntelligenceLevel { get; set; }

    public DateTime? AssessmentDate { get; set; }

    public string? Summary { get; set; }

    public virtual Child Child { get; set; } = null!;

    public virtual IntelligenceType Intelligence { get; set; } = null!;
}
