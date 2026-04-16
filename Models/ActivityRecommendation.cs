using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class ActivityRecommendation
{
    public Guid RecommendationId { get; set; }

    public string? EducatorId { get; set; }

    public string? ChildId { get; set; }

    public Guid? GroupId { get; set; }

    public Guid? ClassId { get; set; }

    public string? ActivityName { get; set; }

    public string? Category { get; set; }

    public string? ActivityDescription { get; set; }

    public int? Duration { get; set; }

    public virtual Child? Child { get; set; }

    public virtual Class? Class { get; set; }

    public virtual Educator? Educator { get; set; }

    public virtual ChildGroup? Group { get; set; }
}
