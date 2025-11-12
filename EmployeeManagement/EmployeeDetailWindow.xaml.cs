using DocumentFormat.OpenXml.Spreadsheet;
using Models.Entities;
using Models.Repositories;
using System.Windows;
using ViewModels;
using ViewModels.Helper;

namespace EmployeeManagement
{
    public partial class EmployeeDetailWindow : Window
    {
        private readonly EmployeeDetailViewModel _viewModel;
        private readonly EmployeeManagementViewModel _mainVM;
        public EmployeeDetailWindow(Employee employee, EmployeeManagementViewModel mainVM)
        {
            InitializeComponent();
<<<<<<< Updated upstream

            var repo = new EmployeeRepository(new EmployeeManagementContext(), 1);
            _viewModel = new EmployeeDetailViewModel(repo, employee);
=======
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("Chưa có người dùng đăng nhập!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            var repo = new EmployeeRepository(new EmployeeManagementContext());
            var logRepo = new ActivityLogRepository(new EmployeeManagementContext());
            _viewModel = new EmployeeDetailViewModel(repo, logRepo, Session.CurrentUser.AccountId, employee);
>>>>>>> Stashed changes

            DataContext = _viewModel;
            _mainVM = mainVM;

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
            var editWindow = new AddEmployeeWindow(_viewModel.Employee);
            if (editWindow.ShowDialog() == true)
            {
<<<<<<< Updated upstream
                // Nếu sửa xong, reload dữ liệu từ DB
                var repo = new EmployeeRepository(new EmployeeManagementContext(), 1);
                _viewModel.Employee = repo.GetById(_viewModel.Employee.EmployeeId);
=======
                // Update Employee trong DB
                var repo = new EmployeeRepository(new EmployeeManagementContext());
                var logRepo = new ActivityLogRepository(new EmployeeManagementContext());
                _viewModel.UpdateEmployee();

                // Reload Employee trong collection ObservableCollection
                _mainVM.ApplyFilter(); // _mainVM là EmployeeManagementViewModel truyền từ main window
>>>>>>> Stashed changes
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
