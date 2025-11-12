using Models.Entities;
using Models.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeManagement
{
    public partial class TimesheetManagementWindow : Window
    {
        private readonly EmployeeManagementContext _context;
        private readonly TimesheetRepository _timesheetRepo;
        private readonly DepartmentRepository _departmentRepo;

        // Constructor
        public TimesheetManagementWindow()
        {
            InitializeComponent();
            _context = new EmployeeManagementContext();
            _timesheetRepo = new TimesheetRepository(_context);
            _departmentRepo = new DepartmentRepository(_context);

            this.Loaded += ManagementWindow_Loaded;
        }

        // Khi cửa sổ được tải
        private async void ManagementWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDepartments(); 
            await LoadFilteredData(); 
        }

        private async Task LoadDepartments()
        {
            var departments = await _departmentRepo.GetAllAsync();
            departments.Insert(0, new Department { DepartmentId = 0, DepartmentName = "-- Tất cả phòng ban --" });
            DepartmentComboBoxFilter.ItemsSource = departments;
            DepartmentComboBoxFilter.SelectedIndex = 0; 
        }

        private async Task LoadFilteredData()
        {
            DateOnly? selectedDate = DatePickerFilter.SelectedDate.HasValue
                ? DateOnly.FromDateTime(DatePickerFilter.SelectedDate.Value)
                : null;

            int? selectedDeptId = (int?)DepartmentComboBoxFilter.SelectedValue;
            string? empName = EmployeeNameTextBoxFilter.Text;

            var timesheets = await _timesheetRepo.GetFilteredTimesheetsAsync(selectedDate, selectedDeptId, empName);

            TimesheetsDataGrid.ItemsSource = timesheets;
        }

        private async void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadFilteredData();
        }

        private async void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            DatePickerFilter.SelectedDate = null;
            DepartmentComboBoxFilter.SelectedIndex = 0;
            EmployeeNameTextBoxFilter.Text = string.Empty;

            await LoadFilteredData();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTimesheet = TimesheetsDataGrid.SelectedItem as Timesheet;
            if (selectedTimesheet == null)
            {
                MessageBox.Show("Vui lòng chọn một bản ghi chấm công để xóa.", "Chưa chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc muốn xóa bản ghi chấm công ngày {selectedTimesheet.WorkDate} của nhân viên {selectedTimesheet.Employee.FullName}?",
                                         "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await _timesheetRepo.DeleteAsync(selectedTimesheet);
                await LoadFilteredData(); 
                MessageBox.Show("Đã xóa thành công.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTimesheet = TimesheetsDataGrid.SelectedItem as Timesheet;
            if (selectedTimesheet == null)
            {
                MessageBox.Show("Vui lòng chọn một bản ghi chấm công để sửa.", "Chưa chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var timesheetToEdit = await _timesheetRepo.GetByIdAsync(selectedTimesheet.TimesheetId);
            if (timesheetToEdit == null)
            {
                MessageBox.Show("Không tìm thấy bản ghi gốc. Có thể đã bị xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var editWindow = new TimesheetEditWindow(timesheetToEdit);
            bool? dialogResult = editWindow.ShowDialog();

            if (dialogResult == true)
            {
                var updatedTimesheet = editWindow.GetUpdatedTimesheet();
                await _timesheetRepo.UpdateCheckOutAsync(updatedTimesheet); 
                await LoadFilteredData(); 
            }
        }
        private void MonthlyReport_Click(object sender, RoutedEventArgs e)
        {
            var reportWindow = new MonthlyReportWindow();
            reportWindow.ShowDialog();
        }
    }
}