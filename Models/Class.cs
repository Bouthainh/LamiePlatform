using System;
using System.Collections.Generic;

namespace BadeePlatform.Models;

public partial class Class
{
    public Guid ClassId { get; set; }

    public string? EducatorId { get; set; }

    public Guid? GradeId { get; set; }

    public string? ClassName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ActivityRecommendation> ActivityRecommendations { get; set; } = new List<ActivityRecommendation>();

    public virtual ICollection<Child> Children { get; set; } = new List<Child>();

    public virtual Educator? Educator { get; set; }

    public virtual Grade? Grade { get; set; }
    public virtual ICollection<ChildGroup> ChildGroups { get; set; } = new List<ChildGroup>();
}
