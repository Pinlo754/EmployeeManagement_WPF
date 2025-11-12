using ClosedXML.Excel;
using Models.Entities;
using Models.Repositories;
using System.Windows;
using ViewModels;
using Microsoft.Win32;
using System.Linq;

namespace EmployeeManagement
{
    public partial class PayrollManagementWindow : Window
    {
        private readonly EmployeeManagementContext _context;
        private readonly PayrollRepository _payrollRepo;
        private readonly EmployeeRepository _employeeRepo;
        private readonly PayrollViewModel _viewModel;

        public PayrollManagementWindow()
        {
            InitializeComponent();

            _context = new EmployeeManagementContext();
            _payrollRepo = new PayrollRepository(_context);
            _employeeRepo = new EmployeeRepository(_context);

            _viewModel = new PayrollViewModel(_payrollRepo, _employeeRepo);
            DataContext = _viewModel;

            // Gắn event ShowMessage (thay cho MessageBox)
            _viewModel.ShowMessage += message =>
            {
                MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            };
        }

        private void AddPayroll_Click(object sender, RoutedEventArgs e)
        {
            var form = new PayrollFormWindow(_context);
            form.Owner = this;
            var result = form.ShowDialog();

            if (result == true)
                _viewModel.LoadPayrolls();
        }

        private void UpdatePayroll_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedPayroll == null)
            {
                MessageBox.Show("Vui lòng chọn bảng lương để chỉnh sửa.");
                return;
            }

            var form = new PayrollFormWindow(_context, _viewModel.SelectedPayroll);
            form.Owner = this;
            var result = form.ShowDialog();

            if (result == true)
                _viewModel.LoadPayrolls();
        }

        private void DeletePayroll_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedPayroll == null)
            {
                MessageBox.Show("Vui lòng chọn bảng lương để xóa.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa bảng lương này?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _payrollRepo.Delete(_viewModel.SelectedPayroll.PayrollId);
                _viewModel.LoadPayrolls();
                MessageBox.Show("Đã xóa bảng lương thành công!");
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PayrollViewModel vm)
            {
                vm.SelectedEmployee = null;
                vm.SelectedMonth = null;
                vm.SelectedQuarter = null;
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Payrolls == null || !_viewModel.Payrolls.Any())
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Mở SaveFileDialog để chọn nơi lưu
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = "BangLuong.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Bảng Lương");

                    // Tiêu đề cột
                    worksheet.Cell(1, 1).Value = "ID";
                    worksheet.Cell(1, 2).Value = "Mã NV";
                    worksheet.Cell(1, 3).Value = "Họ tên";
                    worksheet.Cell(1, 4).Value = "Lương cơ bản";
                    worksheet.Cell(1, 5).Value = "Phụ cấp";
                    worksheet.Cell(1, 6).Value = "Thưởng";
                    worksheet.Cell(1, 7).Value = "Phạt";
                    worksheet.Cell(1, 8).Value = "Tổng thu nhập";
                    worksheet.Cell(1, 9).Value = "Ngày trả";

                    int row = 2;
                    foreach (var p in _viewModel.Payrolls)
                    {
                        worksheet.Cell(row, 1).Value = p.PayrollId;
                        worksheet.Cell(row, 2).Value = p.EmployeeId;
                        worksheet.Cell(row, 3).Value = p.Employee?.FullName;
                        worksheet.Cell(row, 4).Value = p.BaseSalary;
                        worksheet.Cell(row, 5).Value = p.Allowances;
                        worksheet.Cell(row, 6).Value = p.Bonuses;
                        worksheet.Cell(row, 7).Value = p.Penalties;
                        worksheet.Cell(row, 8).Value = p.TotalIncome;
                        worksheet.Cell(row, 9).Value = p.PayDate?.ToString("dd/MM/yyyy"); // nếu dùng DateOnly?
                        row++;
                    }

                    worksheet.Columns().AdjustToContents(); // tự động rộng cột
                    workbook.SaveAs(saveFileDialog.FileName);

                    MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
