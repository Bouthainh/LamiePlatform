using System;
using System.Collections.Generic;

namespace BadeePlatform.Models;

public partial class ChildGroup
{
    public Guid ChildGroupId { get; set; }

    public Guid? ClassId { get; set; }

    public string? GroupName { get; set; }

    public int? MatchScore { get; set; }

    public virtual Class? Class { get; set; }  

    public virtual ICollection<ActivityRecommendation> ActivityRecommendations { get; set; } = new List<ActivityRecommendation>();

    public virtual ICollection<Child> Children { get; set; } = new List<Child>();
}
