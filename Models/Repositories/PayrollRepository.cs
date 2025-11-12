using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Models.Repositories
{
    public class PayrollRepository
    {
        private readonly EmployeeManagementContext _context;
        private readonly ActivityLogRepository _logRepo;
        private readonly int _currentUserId; // ID người thao tác

        public PayrollRepository(EmployeeManagementContext context, int currentUserId)
        {
            _context = context;
            _logRepo = new ActivityLogRepository(context);
            _currentUserId = currentUserId;
        }

        // Lấy tất cả bảng lương
        public IEnumerable<Payroll> GetAll()
        {
            var query = from e in _context.Employees.Include(e => e.Department)
                        join p in _context.Payrolls
                            on e.EmployeeId equals p.EmployeeId into gj
                        from sub in gj.DefaultIfEmpty() // LEFT JOIN
                        orderby sub.PayDate descending
                        select new Payroll
                        {
                            PayrollId = sub != null ? sub.PayrollId : 0,
                            EmployeeId = e.EmployeeId,
                            Employee = e,
                            BaseSalary = sub != null ? sub.BaseSalary : e.BaseSalary,
                            Allowances = sub != null ? sub.Allowances : 0,
                            Bonuses = sub != null ? sub.Bonuses : 0,
                            Penalties = sub != null ? sub.Penalties : 0,
                            TotalIncome = sub != null
                                ? sub.TotalIncome
                                : (e.BaseSalary ?? 0),
                            PayDate = sub != null ? sub.PayDate : null
                        };

            return query.ToList();
        }

        // Lấy bảng lương theo Id
        public Payroll? GetById(int payrollId)
        {
            return _context.Payrolls
                           .Include(p => p.Employee)
                           .FirstOrDefault(p => p.PayrollId == payrollId);
        }

        // Thêm bảng lương
        public void Add(Payroll payroll)
        {
            if (payroll == null) return;

            payroll.TotalIncome = (payroll.BaseSalary ?? 0)
                                + (payroll.Allowances ?? 0)
                                + (payroll.Bonuses ?? 0)
                                - (payroll.Penalties ?? 0);

            _context.Payrolls.Add(payroll);

            if (payroll.EmployeeId != null)
            {
                var emp = _context.Employees.FirstOrDefault(e => e.EmployeeId == payroll.EmployeeId);
                if (emp != null)
                    emp.BaseSalary = payroll.BaseSalary;
            }

            _context.SaveChanges();

            // Ghi log
            _logRepo.LogAction(_currentUserId, "Add", "Payroll", payroll.PayrollId,
                $"Thêm bảng lương cho nhân viên: {payroll.Employee?.FullName} - Tổng thu nhập: {payroll.TotalIncome}");
        }

        // Cập nhật bảng lương
        public void Update(Payroll payroll)
        {
            var existing = GetById(payroll.PayrollId);
            if (existing == null) return;

            existing.BaseSalary = payroll.BaseSalary;
            existing.Allowances = payroll.Allowances;
            existing.Bonuses = payroll.Bonuses;
            existing.Penalties = payroll.Penalties;
            existing.TotalIncome = (payroll.BaseSalary ?? 0)
                                 + (payroll.Allowances ?? 0)
                                 + (payroll.Bonuses ?? 0)
                                 - (payroll.Penalties ?? 0);
            existing.PayDate = payroll.PayDate;

            if (existing.EmployeeId != null)
            {
                var emp = _context.Employees.FirstOrDefault(e => e.EmployeeId == existing.EmployeeId);
                if (emp != null)
                    emp.BaseSalary = existing.BaseSalary;
            }

            _context.SaveChanges();

            // Ghi log
            _logRepo.LogAction(_currentUserId, "Update", "Payroll", payroll.PayrollId,
                $"Cập nhật bảng lương cho nhân viên: {existing.Employee?.FullName} - Tổng thu nhập: {existing.TotalIncome}");
        }

        // Xóa bảng lương
        public void Delete(int payrollId)
        {
            var existing = GetById(payrollId);
            if (existing == null) return;

            _context.Payrolls.Remove(existing);
            _context.SaveChanges();

            // Ghi log
            _logRepo.LogAction(_currentUserId, "Delete", "Payroll", existing.PayrollId,
                $"Xóa bảng lương của nhân viên: {existing.Employee?.FullName} - Tổng thu nhập: {existing.TotalIncome}");
        }
    }
}
