using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Leaf
{
    public int LeaveId { get; set; }

    public int? EmployeeId { get; set; }

    public string? LeaveType { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? DaysCount { get; set; }

    public string? Status { get; set; }

    public virtual Employee? Employee { get; set; }
}
