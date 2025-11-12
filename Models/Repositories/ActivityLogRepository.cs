using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models.Repositories
{
    public class ActivityLogRepository
    {
        private readonly EmployeeManagementContext _context;

        public ActivityLogRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        // Thêm một log cụ thể
        public void Add(ActivityLog log)
        {
            if (log == null) return;

            log.ActionTime = log.ActionTime ?? DateTime.Now; // Nếu chưa có thời gian, đặt mặc định
            _context.ActivityLogs.Add(log);
            _context.SaveChanges();
        }

        // Hàm tiện ích: ghi log nhanh
        public void LogAction(int accountId, string actionType, string targetTable, int? targetId = null, string? description = null)
        {
            var log = new ActivityLog
            {
                AccountId = accountId,
                ActionType = actionType,
                TargetTable = targetTable,
                TargetId = targetId,
                Description = description,
                ActionTime = DateTime.Now
            };

            Add(log);
        }

        // Lấy tất cả log, sắp xếp mới nhất trước
        public List<ActivityLog> GetAll()
        {
            return _context.ActivityLogs
                .OrderByDescending(l => l.ActionTime)
                .ToList();
        }

        // Lấy log theo tài khoản cụ thể
        public List<ActivityLog> GetByAccount(int accountId)
        {
            return _context.ActivityLogs
                .Where(l => l.AccountId == accountId)
                .OrderByDescending(l => l.ActionTime)
                .ToList();
        }
    }
}
