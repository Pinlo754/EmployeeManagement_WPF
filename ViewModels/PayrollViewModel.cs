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

        // Danh sách các tháng và quý
        public ObservableCollection<int> Months { get; set; } = new ObservableCollection<int>(
            Enumerable.Range(1, 12)
        );
        public ObservableCollection<int> Quarters { get; set; } = new ObservableCollection<int> { 1, 2, 3, 4 };

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public event Action<string>? ShowMessage;

        public PayrollViewModel(PayrollRepository payrollRepo, EmployeeRepository employeeRepo)
        {
            _payrollRepo = payrollRepo;
            _employeeRepo = employeeRepo;

            // Load nhân viên
            Employees.Clear();
            foreach (var emp in _employeeRepo.GetAll())
                Employees.Add(emp);

            AddCommand = new RelayCommand(_ => ShowMessage?.Invoke("Nhấn nút Thêm để mở form thêm bảng lương."));
            UpdateCommand = new RelayCommand(_ => ShowMessage?.Invoke("Nhấn nút Sửa để mở form chỉnh sửa bảng lương."), _ => SelectedPayroll != null);
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


        private void DeletePayroll()
        {
            if (SelectedPayroll == null)
            {
                ShowMessage?.Invoke("Vui lòng chọn bảng lương để xóa.");
                return;
            }

            _payrollRepo.Delete(SelectedPayroll.PayrollId);
            LoadPayrolls();
            ShowMessage?.Invoke("Đã xóa bảng lương thành công!");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
