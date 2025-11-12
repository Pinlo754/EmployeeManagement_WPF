using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Repositories
{
    public class TimesheetRepository
    {
        private readonly EmployeeManagementContext _context;

        public TimesheetRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<Timesheet?> GetTodayTimesheetAsync(int employeeId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            return await _context.Timesheets
                .FirstOrDefaultAsync(t => t.EmployeeId == employeeId && t.WorkDate == today);
        }

        public async Task CreateCheckInAsync(Timesheet timesheet)
        {
            _context.Timesheets.Add(timesheet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCheckOutAsync(Timesheet timesheet)
        {
            _context.Timesheets.Update(timesheet);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Timesheet>> GetFilteredTimesheetsAsync(
                    DateOnly? date, int? departmentId, string? employeeName)
        {
            var query = _context.Timesheets
                .Include(t => t.Employee) 
                .ThenInclude(e => e.Department) 
                .OrderByDescending(t => t.WorkDate)
                .ThenBy(t => t.Employee.FullName)
                .AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(t => t.WorkDate == date.Value);
            }

            if (departmentId.HasValue && departmentId.Value > 0)
            {
                query = query.Where(t => t.Employee.DepartmentId == departmentId.Value);
            }

            if (!string.IsNullOrEmpty(employeeName))
            {
                query = query.Where(t => t.Employee.FullName.Contains(employeeName));
            }

            return await query.ToListAsync();
        }

        public async Task<Timesheet?> GetByIdAsync(int timesheetId)
        {
            return await _context.Timesheets
               .Include(t => t.Employee)
               .FirstOrDefaultAsync(t => t.TimesheetId == timesheetId);
        }

        public async Task DeleteAsync(Timesheet timesheet)
        {
            _context.Timesheets.Remove(timesheet);
            await _context.SaveChangesAsync();
        }
    }
}