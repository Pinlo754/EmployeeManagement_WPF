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
    }
}