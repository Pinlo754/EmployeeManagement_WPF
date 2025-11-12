using Models.Entities;
using Models.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ViewModels
{
    public class EmployeeManagementViewModel : INotifyPropertyChanged
    {
        public readonly EmployeeRepository EmployeeRepo;
        public readonly ActivityLogRepository LogRepo;

        private readonly DepartmentRepository _departmentRepo;
        private readonly int _currentUserId;

        private ObservableCollection<Employee> _allEmployees;

        public EmployeeManagementViewModel(EmployeeRepository empRepo,
                                           DepartmentRepository depRepo,
                                           ActivityLogRepository logRepo,
                                           int currentUserId)
        {
            EmployeeRepo = empRepo;
            _departmentRepo = depRepo;
            LogRepo = logRepo;
            _currentUserId = currentUserId;

            // Load tất cả nhân viên
            _allEmployees = new ObservableCollection<Employee>(EmployeeRepo.GetAll() ?? new System.Collections.Generic.List<Employee>());
            Employees = new ObservableCollection<Employee>(_allEmployees);

            // Load phòng ban
            Departments = new ObservableCollection<Department>(_departmentRepo.GetAll() ?? new System.Collections.Generic.List<Department>());

            Genders = new ObservableCollection<string> { "Nam", "Nữ", "Khác" };
        }

        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<Department> Departments { get; set; }
        public ObservableCollection<string> Genders { get; set; }

        private Employee? _selectedEmployee;
        public Employee? SelectedEmployee
        {
            get => _selectedEmployee;
            set { _selectedEmployee = value; OnPropertyChanged(); }
        }

        // ====================
        // FILTER
        // ====================
        private string _searchName = "";
        public string SearchName
        {
            get => _searchName;
            set { _searchName = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private Department? _selectedDepartment;
        public Department? SelectedDepartment
        {
            get => _selectedDepartment;
            set { _selectedDepartment = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private string? _selectedGender;
        public string? SelectedGender
        {
            get => _selectedGender;
            set { _selectedGender = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private decimal? _minSalary;
        public decimal? MinSalary
        {
            get => _minSalary;
            set { _minSalary = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private decimal? _maxSalary;
        public decimal? MaxSalary
        {
            get => _maxSalary;
            set { _maxSalary = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private DateTime? _startDateFrom;
        public DateTime? StartDateFrom
        {
            get => _startDateFrom;
            set { _startDateFrom = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private DateTime? _startDateTo;
        public DateTime? StartDateTo
        {
            get => _startDateTo;
            set { _startDateTo = value; OnPropertyChanged(); ApplyFilter(); }
        }

        public void ApplyFilter()
        {
            var filtered = _allEmployees.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchName))
                filtered = filtered.Where(e => e.FullName.Contains(SearchName, StringComparison.OrdinalIgnoreCase));

            if (SelectedDepartment != null)
                filtered = filtered.Where(e => e.DepartmentId == SelectedDepartment.DepartmentId);

            if (!string.IsNullOrWhiteSpace(SelectedGender))
                filtered = filtered.Where(e => e.Gender == SelectedGender);

            if (MinSalary.HasValue)
                filtered = filtered.Where(e => e.BaseSalary >= MinSalary.Value);

            if (MaxSalary.HasValue)
                filtered = filtered.Where(e => e.BaseSalary <= MaxSalary.Value);

            if (StartDateFrom.HasValue)
                filtered = filtered.Where(e => e.StartDate >= DateOnly.FromDateTime(StartDateFrom.Value));

            if (StartDateTo.HasValue)
                filtered = filtered.Where(e => e.StartDate <= DateOnly.FromDateTime(StartDateTo.Value));

            Employees.Clear();
            foreach (var emp in filtered)
                Employees.Add(emp);
        }

        public void ClearFilter()
        {
            SearchName = "";
            SelectedDepartment = null;
            SelectedGender = null;
            MinSalary = null;
            MaxSalary = null;
            StartDateFrom = null;
            StartDateTo = null;
            ApplyFilter();
        }

        // ====================
        // CRUD + Log
        // ====================
        public void AddEmployee(Employee emp)
        {
            if (emp == null) return;
            EmployeeRepo.Add(emp);
            _allEmployees.Add(emp);
            ApplyFilter();
            LogRepo.LogAction(_currentUserId, "Add", "Employee", emp.EmployeeId, $"Thêm nhân viên {emp.FullName}");
        }

        public void EditEmployee(Employee emp)
        {
            if (emp == null) return;

            var existing = EmployeeRepo.GetById(emp.EmployeeId);
            if (existing != null)
            {
                existing.FullName = emp.FullName;
                existing.DepartmentId = emp.DepartmentId;
                existing.Gender = emp.Gender;
                existing.BaseSalary = emp.BaseSalary;
                existing.StartDate = emp.StartDate;

                EmployeeRepo.Update(existing);
                ApplyFilter();
                LogRepo.LogAction(_currentUserId, "Update", "Employee", existing.EmployeeId, $"Cập nhật nhân viên {existing.FullName}");
            }
        }

        public void DeleteEmployee(Employee emp)
        {
            if (emp == null) return;

            var existing = EmployeeRepo.GetById(emp.EmployeeId);
            if (existing != null)
            {
                EmployeeRepo.Delete(existing.EmployeeId);
                _allEmployees.Remove(existing);
                ApplyFilter();
                LogRepo.LogAction(_currentUserId, "Delete", "Employee", existing.EmployeeId, $"Xóa nhân viên {existing.FullName}");
            }
        }

        // ====================
        // PropertyChanged
        // ====================
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
