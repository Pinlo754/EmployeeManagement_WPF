using Models.Entities;
using Models.Repositories;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ViewModels;
using ViewModels.Helper;

namespace EmployeeManagement
{
    public partial class AddEmployeeWindow : Window
    {
        public Employee Employee => _viewModel.Employee;

        private readonly EmployeeFormViewModel _viewModel;

        public AddEmployeeWindow(Employee? employee = null)
        {
            InitializeComponent();

            // Khởi tạo repository phòng ban để load ComboBox
            var deptRepo = new DepartmentRepository(new EmployeeManagementContext());
            _viewModel = new EmployeeFormViewModel(employee);

            // Nếu đang sửa, chọn phòng ban hiện tại
            if (employee?.DepartmentId != null)
            {
                _viewModel.SelectedDepartment = _viewModel.Departments
                    .FirstOrDefault(d => d.DepartmentId == employee.DepartmentId);
            }

            DataContext = _viewModel;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = ValidateInput();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _viewModel.PrepareAccount(); // gán username/password

            // Gọi trực tiếp Update vào DB
            var empRepo = new EmployeeRepository(new EmployeeManagementContext());
            var logRepo = new ActivityLogRepository(new EmployeeManagementContext());
            _viewModel.UpdateEmployee(empRepo, logRepo, Session.CurrentUser.AccountId);

            this.DialogResult = true;
            this.Close();
        }


        private string ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(_viewModel.FullName))
                return "Họ tên không được để trống!";

            if (!_viewModel.DateOfBirthPicker.HasValue)
                return "Vui lòng chọn ngày sinh!";
            if (_viewModel.DateOfBirthPicker.Value > DateTime.Now)
                return "Ngày sinh không thể lớn hơn ngày hiện tại!";


            if (string.IsNullOrWhiteSpace(_viewModel.Gender))
                return "Vui lòng chọn giới tính!";

            if (_viewModel.SelectedDepartment == null)
                return "Vui lòng chọn phòng ban!";

            if (!string.IsNullOrWhiteSpace(_viewModel.Email))
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(_viewModel.Email, pattern))
                    return "Email không hợp lệ!";
            }

            if (!string.IsNullOrWhiteSpace(_viewModel.Phone))
            {
                string phonePattern = @"^\+?\d{9,15}$";
                if (!Regex.IsMatch(_viewModel.Phone, phonePattern))
                    return "Số điện thoại không hợp lệ!";
            }

            if (_viewModel.BaseSalary.HasValue && _viewModel.BaseSalary.Value < 0)
                return "Lương cơ bản phải >= 0!";

            return string.Empty;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
