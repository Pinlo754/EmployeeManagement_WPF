using ClosedXML.Excel;
using Microsoft.Win32;
using Models.Entities;
using Models.Repositories;
using System;
using System.Linq;
using System.Windows;
using ViewModels;
using ViewModels.Helper;

namespace EmployeeManagement
{
    public partial class EmployeeManagementWindow : Window
    {
        private readonly EmployeeManagementViewModel _viewModel;

        public EmployeeManagementWindow()
        {
            InitializeComponent();
<<<<<<< Updated upstream

            var empRepo = new EmployeeRepository(new EmployeeManagementContext(), 1);
=======
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("Chưa có người dùng đăng nhập!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            var empRepo = new EmployeeRepository(new EmployeeManagementContext());
>>>>>>> Stashed changes
            var depRepo = new DepartmentRepository(new EmployeeManagementContext());
            var logRepo = new ActivityLogRepository(new EmployeeManagementContext());
            _viewModel = new EmployeeManagementViewModel(empRepo, depRepo, logRepo, Session.CurrentUser.AccountId);

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

        private void ImportExcel_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Workbook|*.xlsx;*.xls",
                Title = "Chọn file Excel chứa danh sách nhân viên"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using var workbook = new ClosedXML.Excel.XLWorkbook(openFileDialog.FileName);
                    var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên
                    var rows = worksheet.RowsUsed().Skip(1); // Bỏ qua header

                    foreach (var row in rows)
                    {
                        var emp = new Employee
                        {
                            FullName = row.Cell(2).GetString(),
                            DateOfBirth = DateOnly.TryParse(row.Cell(3).GetString(), out var dob) ? dob : null,
                            Gender = row.Cell(4).GetString(),
                            DepartmentId = _viewModel.Departments.FirstOrDefault(d => d.DepartmentName == row.Cell(5).GetString())?.DepartmentId ?? 0,
                            Position = row.Cell(6).GetString(),
                            BaseSalary = decimal.TryParse(row.Cell(7).GetString(), out var salary) ? salary : 0,
                            Phone = row.Cell(8).GetString(),
                            Address = row.Cell(9).GetString(),
                            StartDate = DateOnly.TryParse(row.Cell(10).GetString(), out var sd) ? sd : null
                        };

                        _viewModel.AddEmployee(emp);
                    }

                    MessageBox.Show("Import Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi nhập Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
