using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;
using System;

namespace Models.Repositories
{
    public class LeaveRepository
    {
        private readonly EmployeeManagementContext _context;

        public LeaveRepository(EmployeeManagementContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Leaf leaf)
        {
            _context.Leaves.Add(leaf);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Leaf>> GetAllAsync()
        {
            return await _context.Leaves
                .Include(l => l.Employee) 
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<Leaf?> GetByIdAsync(int leafId)
        {
            return await _context.Leaves.FindAsync(leafId);
        }

        public async Task UpdateAsync(Leaf leaf)
        {
            _context.Leaves.Update(leaf);
            await _context.SaveChangesAsync();
        }
        public async Task<List<EmployeeLeaveSummaryView>> GetEmployeeLeaveSummaryAsync(int totalAllowedDays)
        {
            int currentYear = DateTime.Now.Year;

            var summary = await _context.Employees
                .Select(e => new EmployeeLeaveSummaryView
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.FullName,
                    TotalAllowed = totalAllowedDays,

                    TotalTaken = (int)e.Leaves
                        .Where(l => l.Status == "Approved" &&
                                    l.StartDate.Value.Year == currentYear)
                        .Sum(l => l.DaysCount ?? 0),

                    RemainingDays = totalAllowedDays - (int)e.Leaves
                        .Where(l => l.Status == "Approved" &&
                                    l.StartDate.Value.Year == currentYear)
                        .Sum(l => l.DaysCount ?? 0)
                })
                .OrderBy(s => s.EmployeeName)
                .ToListAsync();

            return summary;
        }
        public async Task<int> GetApprovedLeaveDaysTakenAsync(int employeeId)
        {
            int currentYear = DateTime.Now.Year;

            var totalDaysTaken = await _context.Leaves
                .Where(l => l.EmployeeId == employeeId &&
                            l.Status == "Approved" &&
                            l.StartDate.Value.Year == currentYear)
                .SumAsync(l => l.DaysCount ?? 0); 

            return (int)totalDaysTaken;
        }
    }
}