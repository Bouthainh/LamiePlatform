using System;
using System.Collections.Generic;

namespace LamiePlatform.Models;

public partial class GameLevel
{
    public Guid LevelId { get; set; }

    public string? LevelName { get; set; }

    public string? LevelDescription { get; set; }

    public Guid? IntelligenceId { get; set; }

    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();

    public virtual IntelligenceType? Intelligence { get; set; }
}