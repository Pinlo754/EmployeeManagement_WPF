using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Timesheet
{
    public int TimesheetId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly? WorkDate { get; set; }

    public TimeOnly? CheckIn { get; set; }

    public TimeOnly? CheckOut { get; set; }

    public decimal? HoursWorked { get; set; }

    public decimal? OvertimeHours { get; set; }

    public string? LeaveType { get; set; }

    public string? Status { get; set; }

    public virtual Employee? Employee { get; set; }
}
