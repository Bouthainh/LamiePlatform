using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class Child
{
    public string ChildId { get; set; } = null!;

    public Guid? SchoolId { get; set; }

    public Guid? ChildGroupId { get; set; }

    public Guid? ClassId { get; set; }

    public Guid? GradeId { get; set; }

    public string? ChildName { get; set; }

    public string? Gender { get; set; }

    public int? Age { get; set; }

    public string? LoginCode { get; set; }


    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ActivityRecommendation> ActivityRecommendations { get; set; } = new List<ActivityRecommendation>();


    public virtual ChildGroup? ChildGroup { get; set; }

    public virtual ICollection<ChildIntelligence> ChildIntelligences { get; set; } = new List<ChildIntelligence>();

    public virtual Class? Class { get; set; }

    public virtual ICollection<EducatorPermission> EducatorPermissions { get; set; } = new List<EducatorPermission>();

    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();

    public virtual Grade? Grade { get; set; }

    public virtual ICollection<ParentChild> ParentChildren { get; set; } = new List<ParentChild>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual School? School { get; set; }
}
