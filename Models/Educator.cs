using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class Educator
{
    public string EducatorId { get; set; } = null!;

    public Guid? SchoolId { get; set; }

    public string? EducatorName { get; set; }

    public string? Email { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? PhoneNumber { get; set; }

    public bool? IsVerified { get; set; }

    public virtual ICollection<ActivityRecommendation> ActivityRecommendations { get; set; } = new List<ActivityRecommendation>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<EducatorPermission> EducatorPermissions { get; set; } = new List<EducatorPermission>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual School? School { get; set; }
}
