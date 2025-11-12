using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;


namespace Models.Repositories
{
    public class ReportRepository
    {
        private readonly EmployeeManagementContext _context;

        public ReportRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<List<MonthlyReportView>> GetMonthlyReportAsync(int month, int year)
        {
            var report = await _context.Employees
                .Include(e => e.Department) 

                .Select(e => new MonthlyReportView
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.FullName,
                    DepartmentName = e.Department.DepartmentName,

                    TotalWorkDays = e.Timesheets
                        .Count(t => t.WorkDate.Value.Month == month &&
                                    t.WorkDate.Value.Year == year &&
                                    t.Status == "Approved"), 

                    TotalLeaveDays = (int)e.Leaves
                        .Where(l => l.Status == "Approved" && 
                                    l.StartDate.Value.Month == month &&
                                    l.StartDate.Value.Year == year)
                        .Sum(l => l.DaysCount ?? 0), 

                    TotalOTHours = e.Timesheets
                        .Where(t => t.WorkDate.Value.Month == month &&
                                    t.WorkDate.Value.Year == year)
                        .Sum(t => t.OvertimeHours ?? 0) 
                })
                .Where(r => r.TotalWorkDays > 0 || r.TotalLeaveDays > 0 || r.TotalOTHours > 0) 
                .OrderBy(r => r.DepartmentName)
                .ThenBy(r => r.EmployeeName)
                .ToListAsync();

            return report;
        }
    }
}