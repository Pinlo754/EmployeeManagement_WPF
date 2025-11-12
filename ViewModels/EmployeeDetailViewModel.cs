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
        public event Action? EmployeeDeleted;
        public EmployeeDetailViewModel(EmployeeRepository repo, Employee employee)
        {
            _repo = repo;
            Employee = employee;
        }

        private Employee _employee;
        public Employee Employee
        {
            get => _employee;
            set { _employee = value; OnPropertyChanged(); }
        }

        // Event để hiển thị thông báo, lỗi hoặc xác nhận
        public event Action<string>? ShowMessage;

        // Event yêu cầu xác nhận xóa
        public event Func<string, bool>? ConfirmDelete;

        public bool DeleteEmployee()
        {
            if (Employee == null) return false;

            // Kiểm tra xem View có trả về true khi xác nhận xóa
            bool confirmed = ConfirmDelete?.Invoke($"Bạn có chắc muốn xóa nhân viên {Employee.FullName}?") ?? false;

            if (!confirmed) return false;

            try
            {
                // Lấy employee đầy đủ với các navigation property để xóa an toàn
                var emp = _repo.GetById(Employee.EmployeeId);
                if (emp == null) return false;

                // Nếu cần, xóa các collection liên quan
                emp.Leaves.Clear();
                emp.Payrolls.Clear();
                emp.Timesheets.Clear();

                _repo.Delete(emp.EmployeeId);

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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
