using Models.Entities;
using Models.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ViewModels
{
    public class EmployeeFormViewModel : INotifyPropertyChanged
    {
        private readonly DepartmentRepository _departmentRepo;

        public EmployeeFormViewModel(Employee? employee = null)
        {
            Employee = employee ?? new Employee();

            _departmentRepo = new DepartmentRepository(new EmployeeManagementContext());
            Departments = new ObservableCollection<Department>(_departmentRepo.GetAll());

            if (Employee.DepartmentId.HasValue)
                SelectedDepartment = Departments.FirstOrDefault(d => d.DepartmentId == Employee.DepartmentId.Value);

            if (Employee.Account != null)
                Username = Employee.Account.Username;
        }

        public Employee Employee { get; set; }

        // -------------------------------
        // Username / Password
        private string _username = "";
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        // -------------------------------
        public string FullName
        {
            get => Employee.FullName;
            set { Employee.FullName = value; OnPropertyChanged(); }
        }

        public DateTime? DateOfBirthPicker
        {
            get => Employee.DateOfBirth.HasValue
                   ? new DateTime(Employee.DateOfBirth.Value.Year, Employee.DateOfBirth.Value.Month, Employee.DateOfBirth.Value.Day)
                   : null;
            set
            {
                Employee.DateOfBirth = value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
                OnPropertyChanged();
            }
        }

        public string? Gender
        {
            get => Employee.Gender;
            set { Employee.Gender = value; OnPropertyChanged(); }
        }

        public string? Address
        {
            get => Employee.Address;
            set { Employee.Address = value; OnPropertyChanged(); }
        }

        public string? Phone
        {
            get => Employee.Phone;
            set { Employee.Phone = value; OnPropertyChanged(); }
        }

        public string? Email
        {
            get => Employee.Email;
            set { Employee.Email = value; OnPropertyChanged(); }
        }

        public string? Position
        {
            get => Employee.Position;
            set { Employee.Position = value; OnPropertyChanged(); }
        }

        public decimal? BaseSalary
        {
            get => Employee.BaseSalary;
            set { Employee.BaseSalary = value; OnPropertyChanged(); }
        }

        public DateTime? StartDatePicker
        {
            get => Employee.StartDate.HasValue
                   ? new DateTime(Employee.StartDate.Value.Year, Employee.StartDate.Value.Month, Employee.StartDate.Value.Day)
                   : (DateTime?)null;
            set
            {
                Employee.StartDate = value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
                OnPropertyChanged();
            }
        }

        public string? AvatarUrl
        {
            get => Employee.AvatarUrl;
            set { Employee.AvatarUrl = value; OnPropertyChanged(); }
        }

        // -------------------------------
        public ObservableCollection<Department> Departments { get; set; }

        private Department? _selectedDepartment;
        public Department? SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                if (value != null) Employee.DepartmentId = value.DepartmentId;
                OnPropertyChanged();
            }
        }

        // -------------------------------
        public void PrepareAccount()
        {
            if (!string.IsNullOrWhiteSpace(Username))
            {
                if (Employee.Account == null)
                    Employee.Account = new Account();

                Employee.Account.Username = Username;

                if (!string.IsNullOrWhiteSpace(Password))
                    Employee.Account.PasswordHash = HashPassword(Password);

                Employee.Account.FullName = Employee.FullName;
                Employee.Account.Role = "Employee";
                Employee.Account.IsActive = true;
                Employee.Account.CreatedAt ??= DateTime.Now;
            }
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        // ================================
        // Cập nhật Employee và log
        // ================================
        public bool UpdateEmployee(EmployeeRepository employeeRepo, ActivityLogRepository logRepo, int currentUserId)
        {
            try
            {
                PrepareAccount(); // Cập nhật account trước khi save

                var existing = employeeRepo.GetById(Employee.EmployeeId);
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

                    employeeRepo.Update(existing);
                    logRepo.LogAction(currentUserId, "Update", "Employee", existing.EmployeeId, $"Cập nhật nhân viên {existing.FullName}");
                }

                return true;
            }
            catch
            {
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
