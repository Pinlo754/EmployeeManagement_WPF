using ClosedXML.Excel;
using Microsoft.Win32;
using Models.Entities;
using Models.Repositories;
using System.Linq;
using System.Windows;
using ViewModels;
using ViewModels.Helper;

namespace EmployeeManagement
{
    public partial class PayrollManagementWindow : Window
    {
        private readonly EmployeeManagementContext _context;
        private readonly PayrollRepository _payrollRepo;
        private readonly EmployeeRepository _employeeRepo;
        private readonly ActivityLogRepository _logRepo;
        private readonly PayrollViewModel _viewModel;

        public PayrollManagementWindow()
        {
            InitializeComponent();
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("Chưa có người dùng đăng nhập!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            _context = new EmployeeManagementContext();
            _payrollRepo = new PayrollRepository(_context);
            _employeeRepo = new EmployeeRepository(_context);
            _logRepo = new ActivityLogRepository(_context);

            _viewModel = new PayrollViewModel(_payrollRepo, _employeeRepo, Session.CurrentUser.AccountId);
            DataContext = _viewModel;

            _viewModel.ShowMessage += msg =>
            {
                MessageBox.Show(msg, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            };
        }

        private void AddPayroll_Click(object sender, RoutedEventArgs e)
        {
            var form = new PayrollFormWindow(_context);
            form.Owner = this;
            if (form.ShowDialog() == true)
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
            if (form.ShowDialog() == true)
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
                _viewModel.DeletePayroll();
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearFilter();
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Payrolls == null || !_viewModel.Payrolls.Any())
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

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
                    var ws = workbook.Worksheets.Add("Bảng Lương");

                    ws.Cell(1, 1).Value = "ID";
                    ws.Cell(1, 2).Value = "Mã NV";
                    ws.Cell(1, 3).Value = "Họ tên";
                    ws.Cell(1, 4).Value = "Lương cơ bản";
                    ws.Cell(1, 5).Value = "Phụ cấp";
                    ws.Cell(1, 6).Value = "Thưởng";
                    ws.Cell(1, 7).Value = "Phạt";
                    ws.Cell(1, 8).Value = "Tổng thu nhập";
                    ws.Cell(1, 9).Value = "Ngày trả";

                    int row = 2;
                    foreach (var p in _viewModel.Payrolls)
                    {
                        ws.Cell(row, 1).Value = p.PayrollId;
                        ws.Cell(row, 2).Value = p.EmployeeId;
                        ws.Cell(row, 3).Value = p.Employee?.FullName;
                        ws.Cell(row, 4).Value = p.BaseSalary;
                        ws.Cell(row, 5).Value = p.Allowances;
                        ws.Cell(row, 6).Value = p.Bonuses;
                        ws.Cell(row, 7).Value = p.Penalties;
                        ws.Cell(row, 8).Value = p.TotalIncome;
                        ws.Cell(row, 9).Value = p.PayDate?.ToString("dd/MM/yyyy");
                        row++;
                    }

                    ws.Columns().AdjustToContents();
                    workbook.SaveAs(saveFileDialog.FileName);

                    MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
