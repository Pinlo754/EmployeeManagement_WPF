using Models.Entities;
using Models.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ViewModels
{
    public class DepartmentManagementViewModel : INotifyPropertyChanged
    {
        private readonly DepartmentRepository _departmentRepo;
        private readonly EmployeeRepository _employeeRepo;

        public ObservableCollection<Department> Departments { get; set; }
        public ObservableCollection<Employee> EmployeesInDepartment { get; set; }

        private Department? _selectedDepartment;
        public Department? SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged();
                LoadEmployeesInDepartment();
            }
        }

        // ================================
        //          CONSTRUCTOR
        // ================================
        public DepartmentManagementViewModel(DepartmentRepository departmentRepo)
        {
            _departmentRepo = departmentRepo;
            _employeeRepo = new EmployeeRepository(new EmployeeManagementContext());
            Departments = new ObservableCollection<Department>(_departmentRepo.GetAll());
            EmployeesInDepartment = new ObservableCollection<Employee>();
        }

        // ================================
        //          THÊM PHÒNG BAN
        // ================================
        public void AddDepartment(Department dep)
        {
            if (dep == null) return;

            _departmentRepo.Add(dep);
            Departments.Add(dep);
            OnPropertyChanged(nameof(Departments));
        }

        // ================================
        //          CẬP NHẬT PHÒNG BAN
        // ================================
        public void UpdateDepartment(Department dep)
        {
            if (dep == null) return;

            _departmentRepo.Update(dep);

            var existing = Departments.FirstOrDefault(d => d.DepartmentId == dep.DepartmentId);
            if (existing != null)
            {
                var index = Departments.IndexOf(existing);
                Departments[index] = dep;
                OnPropertyChanged(nameof(Departments));
            }
        }

        // ================================
        //          XÓA PHÒNG BAN
        // ================================
        public void DeleteDepartment()
        {
            if (SelectedDepartment == null) return;

            _departmentRepo.Delete(SelectedDepartment.DepartmentId);
            Departments.Remove(SelectedDepartment);
            EmployeesInDepartment.Clear();
            SelectedDepartment = null;
            OnPropertyChanged(nameof(Departments));
        }

        // ================================
        //   NẠP DANH SÁCH PHÒNG BAN
        // ================================
        public void LoadDepartments()
        {
            Departments.Clear();
            var list = _departmentRepo.GetAll().ToList();
            foreach (var dep in list)
                Departments.Add(dep);

            OnPropertyChanged(nameof(Departments));
        }

        // ================================
        //   NẠP NHÂN VIÊN THEO PHÒNG BAN
        // ================================
        private void LoadEmployeesInDepartment()
        {
            EmployeesInDepartment.Clear();
            if (SelectedDepartment != null)
            {
                var list = _departmentRepo.GetEmployeesByDepartment(SelectedDepartment.DepartmentId);
                foreach (var emp in list)
                    EmployeesInDepartment.Add(emp);
            }

            OnPropertyChanged(nameof(EmployeesInDepartment));
        }

        // ================================
        //     CẬP NHẬT PHÒNG BAN NHÂN VIÊN
        // ================================
        public void UpdateEmployeeDepartment(Employee emp)
        {
            if (emp == null) return;

            var existing = _employeeRepo.GetById(emp.EmployeeId);
            if (existing != null)
            {
                existing.DepartmentId = emp.DepartmentId;
                _employeeRepo.Update(existing);

                LoadEmployeesInDepartment();
            }
        }


        // ================================
        //     THÔNG BÁO THAY ĐỔI DỮ LIỆU
        // ================================
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
