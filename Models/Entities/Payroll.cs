using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Payroll
{
    public int PayrollId { get; set; }

    public int? EmployeeId { get; set; }

    public decimal? BaseSalary { get; set; }

    public decimal? Allowances { get; set; }

    public decimal? Bonuses { get; set; }

    public decimal? Penalties { get; set; }

    public decimal? TotalIncome { get; set; }

    public DateOnly? PayDate { get; set; }

    public virtual Employee? Employee { get; set; }
}
