using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Models.Repositories
{
    public class DepartmentRepository
    {
        private readonly EmployeeManagementContext _context;

        public DepartmentRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        public IEnumerable<Department> GetAll()
        {
            return _context.Departments
                           .OrderBy(d => d.DepartmentName)
                           .ToList();
        }

        public Department? GetById(int id)
        {
            return _context.Departments.FirstOrDefault(d => d.DepartmentId == id);
        }

        // ➕ Thêm phòng ban
        public void Add(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
        }

        // ✏️ Sửa phòng ban
        public void Update(Department department)
        {
            var existing = _context.Departments.FirstOrDefault(d => d.DepartmentId == department.DepartmentId);
            if (existing != null)
            {
                existing.DepartmentName = department.DepartmentName;
                existing.Description = department.Description;
                _context.SaveChanges();
            }
        }

        // ❌ Xóa phòng ban
        public void Delete(int id)
        {
            var department = _context.Departments.FirstOrDefault(d => d.DepartmentId == id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                _context.SaveChanges();
            }
        }

        // 👥 Lấy danh sách nhân viên trong phòng ban
        public IEnumerable<Employee> GetEmployeesByDepartment(int departmentId)
        {
            return _context.Employees
                           .Where(e => e.DepartmentId == departmentId)
                           .OrderBy(e => e.FullName)
                           .ToList();
        }

        public void UpdateDepartment(int employeeId, int? newDepartmentId)
        {
            var emp = _context.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
            if (emp != null)
            {
                emp.DepartmentId = newDepartmentId;
                _context.SaveChanges();
            }
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _context.Departments.OrderBy(d => d.DepartmentName).ToListAsync();
        }
    }
}
