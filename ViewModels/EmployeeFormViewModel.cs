using Models.Entities;
using Models.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

            // Load username nếu employee có account
            if (Employee.Account != null)
            {
                Username = Employee.Account.Username;
            }
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
                if (value.HasValue)
                    Employee.DateOfBirth = DateOnly.FromDateTime(value.Value);
                else
                    Employee.DateOfBirth = null;
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
                if (value.HasValue)
                    Employee.StartDate = DateOnly.FromDateTime(value.Value);
                else
                    Employee.StartDate = null;
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
            // Demo hash, thực tế nên dùng BCrypt
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
