using Models.Entities;
using Models.Repositories;
using System;
using System.Windows;
using System.Windows.Controls;
using ViewModels;

namespace EmployeeManagement
{
    public partial class DepartmentManagementWindow : Window
    {
        private readonly DepartmentManagementViewModel _viewModel;

        public DepartmentManagementWindow()
        {
            InitializeComponent();

            // Khởi tạo Repository và ViewModel
            var depRepo = new DepartmentRepository(new EmployeeManagementContext());
            _viewModel = new DepartmentManagementViewModel(depRepo);

            DataContext = _viewModel;

            // Tải dữ liệu ban đầu
            _viewModel.LoadDepartments();
        }

        // ======================================
        //            THÊM PHÒNG BAN
        // ======================================
        private void AddDepartment_Click(object sender, RoutedEventArgs e)
        {
            var form = new DepartmentFormWindow
            {
                Title = "Thêm phòng ban mới"
            };

            if (form.ShowDialog() == true)
            {
                var dep = new Department
                {
                    DepartmentName = form.DepartmentName,
                    Description = form.DepartmentDescription,
                    CreatedAt = DateTime.Now
                };

                _viewModel.AddDepartment(dep);
                _viewModel.LoadDepartments();

                MessageBox.Show("Đã thêm phòng ban mới!", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ======================================
        //            SỬA PHÒNG BAN
        // ======================================
        private void EditDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedDepartment == null)
            {
                MessageBox.Show("Vui lòng chọn phòng ban để sửa.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = _viewModel.SelectedDepartment;

            var form = new DepartmentFormWindow
            {
                Title = "Cập nhật phòng ban",
                DepartmentName = selected.DepartmentName,
                DepartmentDescription = selected.Description
            };

            if (form.ShowDialog() == true)
            {
                selected.DepartmentName = form.DepartmentName;
                selected.Description = form.DepartmentDescription;

                _viewModel.UpdateDepartment(selected);
                _viewModel.LoadDepartments();

                MessageBox.Show("Cập nhật phòng ban thành công!", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ======================================
        //            XÓA PHÒNG BAN
        // ======================================
        private void DeleteDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedDepartment == null)
            {
                MessageBox.Show("Vui lòng chọn phòng ban để xóa.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dep = _viewModel.SelectedDepartment;

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa phòng ban '{dep.DepartmentName}' không?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirm == MessageBoxResult.Yes)
            {
                _viewModel.DeleteDepartment();
                _viewModel.LoadDepartments();

                MessageBox.Show("Đã xóa phòng ban thành công!", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ======================================
        //   GÁN NHÂN VIÊN VÀO PHÒNG BAN KHÁC
        // ======================================
        private void DepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox combo && combo.DataContext is Employee emp)
            {
                if (combo.SelectedValue is int newDepartmentId && emp.DepartmentId != newDepartmentId)
                {
                    emp.DepartmentId = newDepartmentId;
                    _viewModel.UpdateEmployeeDepartment(emp);
                }
            }
        }


    }
}
