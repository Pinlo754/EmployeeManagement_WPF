using ClosedXML.Excel;
using Models.Entities;
using Models.Repositories;
using System;
using System.Linq;
using System.Windows;
using ViewModels;
using Microsoft.Win32;

namespace EmployeeManagement
{
    public partial class EmployeeManagementWindow : Window
    {
        private readonly EmployeeManagementViewModel _viewModel;

        public EmployeeManagementWindow()
        {
            InitializeComponent();

            var empRepo = new EmployeeRepository(new EmployeeManagementContext(), 1);
            var depRepo = new DepartmentRepository(new EmployeeManagementContext());

            _viewModel = new EmployeeManagementViewModel(empRepo, depRepo);

            DataContext = _viewModel;
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var form = new AddEmployeeWindow();
            if (form.ShowDialog() == true)
            {
                _viewModel.AddEmployee(form.Employee);
            }
        }

        private void DetailEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedEmployee != null)
            {
                var detailWindow = new EmployeeDetailWindow(_viewModel.SelectedEmployee, _viewModel);
                detailWindow.ShowDialog();
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearFilter();
        }

        #region Export Excel
        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Employees == null || !_viewModel.Employees.Any())
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = "DanhSachNhanVien.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Nhân viên");

                    // Tiêu đề cột
                    string[] headers = { "ID", "Họ tên", "Ngày sinh", "Giới tính", "Phòng ban", "Chức vụ", "Mức lương", "Số điện thoại", "Địa chỉ", "Ngày bắt đầu" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                        worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                    }

                    int row = 2;
                    foreach (var emp in _viewModel.Employees)
                    {
                        worksheet.Cell(row, 1).Value = emp.EmployeeId;
                        worksheet.Cell(row, 2).Value = emp.FullName;
                        worksheet.Cell(row, 3).Value = emp.DateOfBirth?.ToString("dd/MM/yyyy") ?? "";
                        worksheet.Cell(row, 4).Value = emp.Gender;
                        worksheet.Cell(row, 5).Value = emp.Department?.DepartmentName ?? "";
                        worksheet.Cell(row, 6).Value = emp.Position;
                        worksheet.Cell(row, 7).Value = emp.BaseSalary;
                        worksheet.Cell(row, 8).Value = emp.Phone;
                        worksheet.Cell(row, 9).Value = emp.Address;
                        worksheet.Cell(row, 10).Value = emp.StartDate?.ToString("dd/MM/yyyy") ?? "";
                        row++;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs(saveFileDialog.FileName);

                    MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
    }
}
