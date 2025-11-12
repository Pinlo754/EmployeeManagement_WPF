using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public int? TargetDepartmentId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account? CreatedByNavigation { get; set; }

    public virtual Department? TargetDepartment { get; set; }
}
