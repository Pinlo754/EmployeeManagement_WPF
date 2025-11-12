using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Models.Repositories
{
    public class EmployeeRepository
    {
        private readonly EmployeeManagementContext _context;
        private readonly ActivityLogRepository _logRepo;
        private readonly int _currentUserId; 

        public EmployeeRepository(EmployeeManagementContext context)
        {
            _context = context;
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

        public Employee? GetByIdWithDepartment(int id)
        {
            return _context.Employees
                           .Include(e => e.Department)
                           .FirstOrDefault(e => e.EmployeeId == id);
        }


        // Thêm nhân viên mới
        public void Add(Employee employee)
        {
            if (employee == null) return;

            _context.Employees.Add(employee);
            _context.SaveChanges();
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
                existing.Account.CreatedAt = employee.Account.CreatedAt ?? System.DateTime.Now;
            }

            _context.SaveChanges();
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
        }
    }
}
