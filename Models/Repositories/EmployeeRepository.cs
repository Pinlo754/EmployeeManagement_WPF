using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models.Repositories
{
    public class EmployeeRepository
    {
        private readonly EmployeeManagementContext _context;
        private readonly ActivityLogRepository _logRepo;
        private readonly int _currentUserId; // ID người đang thao tác (AccountId)

        public EmployeeRepository(EmployeeManagementContext context, int currentUserId)
        {
            _context = context;
            _logRepo = new ActivityLogRepository(context);
            _currentUserId = currentUserId;
        }

        // Lấy toàn bộ nhân viên
        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees
                           .Include(e => e.Department)
                           .OrderBy(e => e.FullName)
                           .ToList();
        }

        // Lấy nhân viên theo Id
        public Employee? GetById(int employeeId)
        {
            return _context.Employees
                           .Include(e => e.Department)
                           .FirstOrDefault(e => e.EmployeeId == employeeId);
        }

        // Thêm nhân viên mới
        public void Add(Employee employee)
        {
            if (employee == null) return;

            _context.Employees.Add(employee);
            _context.SaveChanges();

            // Ghi log
            _logRepo.LogAction(_currentUserId, "Add", "Employee", employee.EmployeeId, $"Thêm nhân viên: {employee.FullName}");
        }

        // Cập nhật nhân viên
        public void Update(Employee employee)
        {
            var existing = GetById(employee.EmployeeId);
            if (existing == null) return;

            existing.FullName = employee.FullName;
            existing.DateOfBirth = employee.DateOfBirth;
            existing.Gender = employee.Gender;
            existing.Address = employee.Address;
            existing.Phone = employee.Phone;
            existing.Email = employee.Email;
            existing.DepartmentId = employee.DepartmentId;
            existing.Position = employee.Position;
            existing.BaseSalary = employee.BaseSalary;
            existing.StartDate = employee.StartDate;
            existing.AvatarUrl = employee.AvatarUrl;

            // Update Account nếu có
            if (employee.Account != null)
            {
                if (existing.Account == null)
                    existing.Account = new Account();

                existing.Account.Username = employee.Account.Username;
                existing.Account.PasswordHash = employee.Account.PasswordHash;
                existing.Account.FullName = employee.Account.FullName;
                existing.Account.Role = employee.Account.Role;
                existing.Account.IsActive = employee.Account.IsActive;
                existing.Account.CreatedAt = employee.Account.CreatedAt ?? DateTime.Now;
            }

            _context.SaveChanges();

            // Ghi log
            _logRepo.LogAction(_currentUserId, "Update", "Employee", employee.EmployeeId, $"Cập nhật nhân viên: {employee.FullName}");
        }

        // Xóa nhân viên
        public void Delete(int employeeId)
        {
            var employee = GetById(employeeId);
            if (employee == null) return;

            if (employee.Payrolls != null && employee.Payrolls.Any())
            {
                _context.Payrolls.RemoveRange(employee.Payrolls);
            }

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            // Ghi log
            _logRepo.LogAction(_currentUserId, "Delete", "Employee", employee.EmployeeId, $"Xóa nhân viên: {employee.FullName}");
        }
    }
}
