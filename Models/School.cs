using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class School
{
    public Guid SchoolId { get; set; }

    public string? SchoolName { get; set; }

    public string? City { get; set; }

    public string? Branch { get; set; }

    public virtual ICollection<Child> Children { get; set; } = new List<Child>();

    public virtual ICollection<Educator> Educators { get; set; } = new List<Educator>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
