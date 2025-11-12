using Models.Entities;
using Models.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ViewModels.Helper;

namespace ViewModels
{
    public class PayrollFormViewModel : INotifyPropertyChanged
    {
        private readonly PayrollRepository _payrollRepo;
        private readonly EmployeeRepository _employeeRepo;

        public Payroll Payroll { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public event Action<string>? ShowMessage;
        public event Action<bool>? RequestClose;

        public PayrollFormViewModel(PayrollRepository payrollRepo, EmployeeRepository employeeRepo, Payroll? payroll = null)
        {
            _payrollRepo = payrollRepo;
            _employeeRepo = employeeRepo;

            Employees = new ObservableCollection<Employee>(_employeeRepo.GetAll());
            Payroll = payroll ?? new Payroll { PayDate = DateOnly.FromDateTime(DateTime.Now) };

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private void Save()
        {
            if (Payroll.EmployeeId == null || Payroll.EmployeeId == 0)
            {
                ShowMessage?.Invoke("Vui lòng chọn nhân viên!");
                return;
            }

            Payroll.TotalIncome = (Payroll.BaseSalary ?? 0) + (Payroll.Allowances ?? 0)
                                + (Payroll.Bonuses ?? 0) - (Payroll.Penalties ?? 0);

            if (Payroll.PayrollId == 0)
                _payrollRepo.Add(Payroll);
            else
                _payrollRepo.Update(Payroll);

            ShowMessage?.Invoke("Lưu bảng lương thành công!");
            RequestClose?.Invoke(true);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(false);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
