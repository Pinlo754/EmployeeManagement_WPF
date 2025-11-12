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
        private readonly ActivityLogRepository _logRepo;
        private readonly int _currentUserId;

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

        public DepartmentManagementViewModel(
            DepartmentRepository departmentRepo,
            int currentUserId)
        {
            _departmentRepo = departmentRepo;
            _currentUserId = currentUserId;
            _employeeRepo = new EmployeeRepository(new EmployeeManagementContext());
            _logRepo = new ActivityLogRepository(new EmployeeManagementContext());
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

            // Ghi log
            _logRepo.LogAction(_currentUserId, "Add", "Department", dep.DepartmentId, $"Thêm phòng ban {dep.DepartmentName}");
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

            // Ghi log
            _logRepo.LogAction(_currentUserId, "Update", "Department", dep.DepartmentId, $"Cập nhật phòng ban {dep.DepartmentName}");
        }

        // ================================
        //          XÓA PHÒNG BAN
        // ================================
        public void DeleteDepartment()
        {
            if (SelectedDepartment == null) return;

            var depId = SelectedDepartment.DepartmentId;
            var depName = SelectedDepartment.DepartmentName;

            _departmentRepo.Delete(depId);
            Departments.Remove(SelectedDepartment);
            EmployeesInDepartment.Clear();
            SelectedDepartment = null;
            OnPropertyChanged(nameof(Departments));

            // Ghi log
            _logRepo.LogAction(_currentUserId, "Delete", "Department", depId, $"Xóa phòng ban {depName}");
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
                var oldDeptId = existing.DepartmentId;
                existing.DepartmentId = emp.DepartmentId;
                _employeeRepo.Update(existing);

                LoadEmployeesInDepartment();

                // Ghi log
                _logRepo.LogAction(_currentUserId, "Update", "Employee", emp.EmployeeId,
                    $"Chuyển nhân viên {emp.FullName} từ phòng {oldDeptId} sang phòng {emp.DepartmentId}");
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
