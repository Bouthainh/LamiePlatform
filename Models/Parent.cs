using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class Parent
{
    public string ParentId { get; set; } = null!;

    public string? ParentName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public bool? IsVerified { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<EducatorPermission> EducatorPermissions { get; set; } = new List<EducatorPermission>();

    public virtual ICollection<ParentChild> ParentChildren { get; set; } = new List<ParentChild>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
