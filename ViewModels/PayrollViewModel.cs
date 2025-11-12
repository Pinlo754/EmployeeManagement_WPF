using Models.Entities;
using Models.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ViewModels
{
    public class PayrollViewModel : INotifyPropertyChanged
    {
        private readonly PayrollRepository _repository;

        public ObservableCollection<Payroll> Payrolls { get; set; } = new ObservableCollection<Payroll>();

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

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public PayrollViewModel()
        {
            LoadPayrolls();

            AddCommand = new RelayCommand(_ => AddPayroll());
            UpdateCommand = new RelayCommand(_ => UpdatePayroll(), _ => SelectedPayroll != null);
            DeleteCommand = new RelayCommand(_ => DeletePayroll(), _ => SelectedPayroll != null);

            // Thêm dữ liệu mẫu (nếu muốn)
            var sample = new Payroll
            {
                EmployeeId = 1,
                BaseSalary = 1000,
                Allowances = 200,
                Bonuses = 50,
                Penalties = 0,
                PayDate = System.DateOnly.FromDateTime(System.DateTime.Now)
            };
            SelectedPayroll = sample;
            AddCommand.Execute(null);
        }

        public void LoadPayrolls()
        {
            Payrolls.Clear();
            foreach (var p in _repository.GetAll())
                Payrolls.Add(p);
        }

        private void AddPayroll()
        {
            if (SelectedPayroll != null)
            {
                SelectedPayroll.TotalIncome = (SelectedPayroll.BaseSalary ?? 0)
                                             + (SelectedPayroll.Allowances ?? 0)
                                             + (SelectedPayroll.Bonuses ?? 0)
                                             - (SelectedPayroll.Penalties ?? 0);

                _repository.Add(SelectedPayroll);
                LoadPayrolls();
            }
        }

        private void UpdatePayroll()
        {
            if (SelectedPayroll != null)
            {
                SelectedPayroll.TotalIncome = (SelectedPayroll.BaseSalary ?? 0)
                                             + (SelectedPayroll.Allowances ?? 0)
                                             + (SelectedPayroll.Bonuses ?? 0)
                                             - (SelectedPayroll.Penalties ?? 0);

                _repository.Update(SelectedPayroll);
                LoadPayrolls();
            }
        }

        private void DeletePayroll()
        {
            if (SelectedPayroll != null)
            {
                _repository.Delete(SelectedPayroll.PayrollId);
                LoadPayrolls();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
