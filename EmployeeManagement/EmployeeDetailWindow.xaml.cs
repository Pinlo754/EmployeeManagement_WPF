using Models.Entities;
using Models.Repositories;
using System.Windows;
using ViewModels;

namespace EmployeeManagement
{
    public partial class EmployeeDetailWindow : Window
    {
        private readonly EmployeeDetailViewModel _viewModel;

        public EmployeeDetailWindow(Employee employee, EmployeeManagementViewModel mainVM)
        {
            InitializeComponent();

            var repo = new EmployeeRepository(new EmployeeManagementContext(), 1);
            _viewModel = new EmployeeDetailViewModel(repo, employee);

            DataContext = _viewModel;

            _viewModel.ShowMessage += msg => MessageBlock.Text = msg; // MessageBlock là TextBlock trong XAML
            _viewModel.ConfirmDelete += msg =>
            {
                // Dùng MessageBox hoặc Custom Dialog để xác nhận
                var result = MessageBox.Show(msg, "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                return result == MessageBoxResult.Yes;
            };

            _viewModel.EmployeeDeleted += () =>
            {
                mainVM.Employees.Remove(employee);  // xóa khỏi ObservableCollection
            };
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // Mở AddEmployeeWindow với employee hiện tại để sửa
            var editWindow = new AddEmployeeWindow(_viewModel.Employee);
            if (editWindow.ShowDialog() == true)
            {
                // Nếu sửa xong, reload dữ liệu từ DB
                var repo = new EmployeeRepository(new EmployeeManagementContext(), 1);
                _viewModel.Employee = repo.GetById(_viewModel.Employee.EmployeeId);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
                bool success = _viewModel.DeleteEmployee();
                if (success)
                {
                    MessageBox.Show("Xóa thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); // đóng window sau khi xóa
                }
                else
                {
                    MessageBox.Show("Xóa thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }            
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
