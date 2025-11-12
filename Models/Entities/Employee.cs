using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int? AccountId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? DepartmentId { get; set; }

    public string? Position { get; set; }

    public decimal? BaseSalary { get; set; }

    public DateOnly? StartDate { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Leaf> Leaves { get; set; } = new List<Leaf>();

    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

    public virtual ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();
}
