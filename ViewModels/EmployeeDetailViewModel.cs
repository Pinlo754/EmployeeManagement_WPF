using Models.Entities;
using Models.Repositories;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModels
{
    public class EmployeeDetailViewModel : INotifyPropertyChanged
    {
        private readonly EmployeeRepository _repo;
        private readonly ActivityLogRepository _logRepo;
        private readonly int _currentUserId;

        public event Action? EmployeeDeleted;

        public EmployeeDetailViewModel(EmployeeRepository repo, ActivityLogRepository logRepo, int currentUserId, Employee employee)
        {
            _repo = repo;
            _logRepo = logRepo;
            _currentUserId = currentUserId;
            Employee = employee;
        }

        private Employee _employee;
        public Employee Employee
        {
            get => _employee;
            set
            {
                _employee = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DepartmentName));
            }
        }

        // Property để hiển thị phòng ban
        public string DepartmentName
        {
            get
            {
                if (Employee == null || Employee.Department == null)
                    return "Chưa có phòng ban";
                return Employee.Department.DepartmentName;
            }
        }

        // Event để hiển thị thông báo, lỗi hoặc xác nhận
        public event Action<string>? ShowMessage;

        // Event yêu cầu xác nhận xóa
        public event Func<string, bool>? ConfirmDelete;

        // ================================
        // Xóa nhân viên
        // ================================
        public bool DeleteEmployee()
        {
            if (Employee == null) return false;

            bool confirmed = ConfirmDelete?.Invoke($"Bạn có chắc muốn xóa nhân viên {Employee.FullName}?") ?? false;
            if (!confirmed) return false;

            try
            {
                var emp = _repo.GetByIdWithDepartment(Employee.EmployeeId);

                if (emp == null) return false;

                // Xóa các collection liên quan nếu có
                emp.Leaves.Clear();
                emp.Payrolls.Clear();
                emp.Timesheets.Clear();

                _repo.Delete(emp.EmployeeId);
                _logRepo.LogAction(_currentUserId, "Delete", "Employee", emp.EmployeeId, $"Xóa nhân viên {emp.FullName}");

                ShowMessage?.Invoke("Xóa nhân viên thành công.");
                EmployeeDeleted?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                ShowMessage?.Invoke($"Lỗi khi xóa nhân viên: {ex.Message}");
                return false;
            }
        }

        // ================================
        // Cập nhật nhân viên
        // ================================
        public bool UpdateEmployee()
        {
            if (Employee == null) return false;

            try
            {
                var existing = _repo.GetByIdWithDepartment(Employee.EmployeeId);
                if (existing != null)
                {
                    existing.FullName = Employee.FullName;
                    existing.DepartmentId = Employee.DepartmentId;
                    existing.Gender = Employee.Gender;
                    existing.Address = Employee.Address;
                    existing.Phone = Employee.Phone;
                    existing.Email = Employee.Email;
                    existing.Position = Employee.Position;
                    existing.BaseSalary = Employee.BaseSalary;
                    existing.StartDate = Employee.StartDate;
                    existing.DateOfBirth = Employee.DateOfBirth;
                    existing.AvatarUrl = Employee.AvatarUrl;
                    existing.Account = Employee.Account;

                    _repo.Update(existing);
                    _logRepo.LogAction(_currentUserId, "Update", "Employee", existing.EmployeeId, $"Cập nhật nhân viên {existing.FullName}");

                    ShowMessage?.Invoke("Cập nhật nhân viên thành công.");
                    Employee = existing; // cập nhật lại Employee và DepartmentName
                    return true;
                }

                ShowMessage?.Invoke("Không tìm thấy nhân viên để cập nhật.");
                return false;
            }
            catch (Exception ex)
            {
                ShowMessage?.Invoke($"Lỗi khi cập nhật nhân viên: {ex.Message}");
                return false;
            }
        }

        // ================================
        // PropertyChanged
        // ================================
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
