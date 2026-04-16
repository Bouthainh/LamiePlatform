using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class Grade
{
    public Guid GradeId { get; set; }

    public Guid? SchoolId { get; set; }

    public string? GradeName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Child> Children { get; set; } = new List<Child>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual School? School { get; set; }
}
