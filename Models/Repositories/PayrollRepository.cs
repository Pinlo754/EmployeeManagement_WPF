using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models.Repositories
{
    public class PayrollRepository
    {
        private readonly EmployeeManagementContext _context;

        public PayrollRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        // Lấy tất cả bảng lương, kèm thông tin Employee
        public IEnumerable<Payroll> GetAll()
        {
            return _context.Payrolls
                           .Include(p => p.Employee)
                           .OrderByDescending(p => p.PayDate)
                           .ToList();
        }

        // Lấy bảng lương theo PayrollId
        public Payroll? GetById(int payrollId)
        {
            return _context.Payrolls
                           .Include(p => p.Employee)
                           .FirstOrDefault(p => p.PayrollId == payrollId);
        }

        // Thêm bảng lương mới
        public void Add(Payroll payroll)
        {
            if (payroll == null) return;

            // Tính tổng thu nhập trước khi thêm
            payroll.TotalIncome = (payroll.BaseSalary ?? 0)
                                + (payroll.Allowances ?? 0)
                                + (payroll.Bonuses ?? 0)
                                - (payroll.Penalties ?? 0);

            _context.Payrolls.Add(payroll);
            _context.SaveChanges();
        }

        // Cập nhật bảng lương
        public void Update(Payroll payroll)
        {
            var existing = GetById(payroll.PayrollId);
            if (existing == null) return;

            existing.EmployeeId = payroll.EmployeeId;
            existing.BaseSalary = payroll.BaseSalary;
            existing.Allowances = payroll.Allowances;
            existing.Bonuses = payroll.Bonuses;
            existing.Penalties = payroll.Penalties;
            existing.TotalIncome = (payroll.BaseSalary ?? 0)
                                 + (payroll.Allowances ?? 0)
                                 + (payroll.Bonuses ?? 0)
                                 - (payroll.Penalties ?? 0);
            existing.PayDate = payroll.PayDate;

            _context.SaveChanges();
        }

        // Xóa bảng lương
        public void Delete(int payrollId)
        {
            var existing = GetById(payrollId);
            if (existing == null) return;

            _context.Payrolls.Remove(existing);
            _context.SaveChanges();
        }
    }
}
