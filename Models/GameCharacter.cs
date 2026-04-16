using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class GameCharacter
{
    public Guid CharacterId { get; set; }

    public string? CharacterName { get; set; }

    public string? CharacterDescription { get; set; }

    public virtual ICollection<Child> Children { get; set; } = new List<Child>();
}
