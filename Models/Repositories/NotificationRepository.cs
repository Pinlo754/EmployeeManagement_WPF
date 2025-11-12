using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Repositories
{
    public class NotificationRepository
    {
        private readonly EmployeeManagementContext _context;

        public NotificationRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetNotificationsForEmployeeAsync(int? employeeDepartmentId)
        {
            return await _context.Notifications

                .Include(n => n.CreatedByNavigation)

                .Where(n => n.TargetDepartmentId == null || 
                            n.TargetDepartmentId == employeeDepartmentId) 

                .OrderByDescending(n => n.CreatedAt) 
                .ToListAsync();
        }

        public async Task<List<Notification>> GetAllAdminAsync()
        {
            return await _context.Notifications
                .Include(n => n.CreatedByNavigation) 
                .Include(n => n.TargetDepartment)   
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Notification notification)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}