using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class ActivityLog
{
    public int LogId { get; set; }

    public int? AccountId { get; set; }

    public string? ActionType { get; set; }

    public string? TargetTable { get; set; }

    public int? TargetId { get; set; }

    public DateTime? ActionTime { get; set; }

    public string? Description { get; set; }

    public virtual Account? Account { get; set; }
}
