using Models.Entities;
using Models.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ViewModels
{
    public class PayrollViewModel : INotifyPropertyChanged
    {
        private readonly PayrollRepository _payrollRepo;
        private readonly EmployeeRepository _employeeRepo;
        private readonly ActivityLogRepository _logRepo;
        private readonly int _currentUserId;

        public ObservableCollection<Payroll> Payrolls { get; set; } = new ObservableCollection<Payroll>();
        public ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>();

        private Payroll? _selectedPayroll;
        public Payroll? SelectedPayroll
        {
            get => _selectedPayroll;
            set
            {
                _selectedPayroll = value;
                OnPropertyChanged();
                UpdateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        private Employee? _selectedEmployee;
        public Employee? SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();
                LoadPayrolls(); // Load lại khi filter thay đổi
            }
        }

        private int? _selectedMonth;
        public int? SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                _selectedMonth = value;
                OnPropertyChanged();
                LoadPayrolls();
            }
        }

        private int? _selectedQuarter;
        public int? SelectedQuarter
        {
            get => _selectedQuarter;
            set
            {
                _selectedQuarter = value;
                OnPropertyChanged();
                LoadPayrolls();
            }
        }

        public ObservableCollection<int> Months { get; set; } = new ObservableCollection<int>(Enumerable.Range(1, 12));
        public ObservableCollection<int> Quarters { get; set; } = new ObservableCollection<int> { 1, 2, 3, 4 };

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public event Action<string>? ShowMessage;

        public PayrollViewModel(PayrollRepository payrollRepo, EmployeeRepository employeeRepo, int currentUserId)
        {
            _payrollRepo = payrollRepo;
            _employeeRepo = employeeRepo;

            // Khởi tạo logRepo riêng như các ViewModel khác để chắc chắn log được ghi
            _logRepo = new ActivityLogRepository(new EmployeeManagementContext());
            _currentUserId = currentUserId;

            // Load danh sách nhân viên
            Employees.Clear();
            foreach (var emp in _employeeRepo.GetAll())
                Employees.Add(emp);

            AddCommand = new RelayCommand(_ => AddPayroll(), _ => SelectedEmployee != null);
            UpdateCommand = new RelayCommand(_ => UpdatePayroll(), _ => SelectedPayroll != null);
            DeleteCommand = new RelayCommand(_ => DeletePayroll(), _ => SelectedPayroll != null);

            LoadPayrolls();
        }

        public void LoadPayrolls()
        {
            Payrolls.Clear();
            var list = _payrollRepo.GetAll().AsEnumerable();

            if (SelectedEmployee != null)
                list = list.Where(p => p.EmployeeId == SelectedEmployee.EmployeeId);

            if (SelectedMonth != null)
                list = list.Where(p => p.PayDate.HasValue && p.PayDate.Value.Month == SelectedMonth.Value);

            if (SelectedQuarter != null)
                list = list.Where(p => p.PayDate.HasValue && ((p.PayDate.Value.Month - 1) / 3 + 1) == SelectedQuarter.Value);

            foreach (var p in list)
                Payrolls.Add(p);
        }

        // ================================
        //          Thêm bảng lương
        // ================================
        public void AddPayroll()
        {
            if (SelectedEmployee == null)
            {
                ShowMessage?.Invoke("Vui lòng chọn nhân viên để thêm bảng lương.");
                return;
            }

            var payroll = new Payroll
            {
                EmployeeId = SelectedEmployee.EmployeeId,
                PayDate = DateOnly.FromDateTime(DateTime.Now),
                BaseSalary = 0
            };

            _payrollRepo.Add(payroll);

            // Ghi log ngay sau khi Add
            _logRepo.LogAction(_currentUserId, "Add", "Payroll", payroll.PayrollId,
                $"Thêm bảng lương cho nhân viên {SelectedEmployee.FullName}");

            LoadPayrolls();
            ShowMessage?.Invoke("Đã thêm bảng lương!");
        }

        // ================================
        //        Cập nhật bảng lương
        // ================================
        public void UpdatePayroll()
        {
            if (SelectedPayroll == null) return;

            _payrollRepo.Update(SelectedPayroll);

            // Ghi log ngay sau khi Update
            var empName = Employees.FirstOrDefault(e => e.EmployeeId == SelectedPayroll.EmployeeId)?.FullName;
            _logRepo.LogAction(_currentUserId, "Update", "Payroll", SelectedPayroll.PayrollId,
                $"Cập nhật bảng lương cho nhân viên {empName}");

            LoadPayrolls();
            ShowMessage?.Invoke("Cập nhật bảng lương thành công!");
        }

        // ================================
        //        Xóa bảng lương
        // ================================
        public void DeletePayroll()
        {
            if (SelectedPayroll == null)
            {
                ShowMessage?.Invoke("Vui lòng chọn bảng lương để xóa.");
                return;
            }

            var payrollId = SelectedPayroll.PayrollId;
            var empName = Employees.FirstOrDefault(e => e.EmployeeId == SelectedPayroll.EmployeeId)?.FullName;

            _payrollRepo.Delete(payrollId);

            // Ghi log ngay sau khi Delete
            _logRepo.LogAction(_currentUserId, "Delete", "Payroll", payrollId,
                $"Xóa bảng lương của nhân viên {empName}");

            LoadPayrolls();
            ShowMessage?.Invoke("Đã xóa bảng lương thành công!");
        }

        public void ClearFilter()
        {
            SelectedEmployee = null;
            SelectedMonth = null;
            SelectedQuarter = null;
        }

        // ================================
        //  Thông báo thay đổi dữ liệu
        // ================================
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
