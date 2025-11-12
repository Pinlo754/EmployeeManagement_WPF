using System.Windows;
using ViewModels;
using Models.Entities;
using Models.Repositories;

namespace EmployeeManagement
{
    public partial class PayrollFormWindow : Window
    {
        private readonly PayrollFormViewModel _viewModel;

        public PayrollFormWindow(EmployeeManagementContext context, Payroll? payroll = null)
        {
            InitializeComponent();

            var payrollRepo = new PayrollRepository(context, 1);
            var employeeRepo = new EmployeeRepository(context, 1);

            // Khởi tạo ViewModel, truyền vào context và dữ liệu cần sửa (nếu có)
            _viewModel = new PayrollFormViewModel(payrollRepo, employeeRepo, payroll);

            // Gán DataContext cho ViewModel
            DataContext = _viewModel;

            // Sự kiện hiển thị thông báo (thay cho MessageBox)
            _viewModel.ShowMessage += message =>
            {
                MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            // Sự kiện đóng cửa sổ khi người dùng lưu hoặc hủy
            _viewModel.RequestClose += result =>
            {
                this.DialogResult = result;
                this.Close();
            };
        }
    }
}
