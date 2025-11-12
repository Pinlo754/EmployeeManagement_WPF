using Models.Entities;
using Models.Repositories;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EmployeeManagement
{
    public partial class LeaveRequestFormWindow : Window
    {
        private readonly Employee _employee;
        private readonly LeaveRepository _leaveRepo;

        public LeaveRequestFormWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;
            _leaveRepo = new LeaveRepository(new EmployeeManagementContext());

            this.Loaded += Form_Loaded;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            LeaveTypeComboBox.ItemsSource = new List<string>
            {
                "Nghỉ phép năm",
                "Nghỉ ốm",
                "Nghỉ không lương",
                "Nghỉ thai sản",
                "Khác"
            };
            LeaveTypeComboBox.SelectedIndex = 0; 
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                var startDate = StartDatePicker.SelectedDate.Value;
                var endDate = EndDatePicker.SelectedDate.Value;

                if (endDate < startDate)
                {
                    DaysCountText.Text = "Lỗi: Ngày kết thúc sớm hơn";
                    return;
                }

                int daysCount = (int)(endDate - startDate).TotalDays + 1;
                DaysCountText.Text = daysCount.ToString();
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu và kết thúc.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var startDate = StartDatePicker.SelectedDate.Value;
            var endDate = EndDatePicker.SelectedDate.Value;

            if (endDate < startDate)
            {
                MessageBox.Show("Ngày kết thúc không thể sớm hơn ngày bắt đầu.", "Lỗi logic", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int daysCount = (int)(endDate - startDate).TotalDays + 1;

            var newLeaf = new Leaf
            {
                EmployeeId = _employee.EmployeeId,
                LeaveType = LeaveTypeComboBox.SelectedItem.ToString(),
                StartDate = DateOnly.FromDateTime(startDate),
                EndDate = DateOnly.FromDateTime(endDate),
                DaysCount = daysCount,
                Status = "Pending" 
            };

            try
            {
                await _leaveRepo.AddAsync(newLeaf);
                MessageBox.Show("Đã nộp đơn nghỉ phép thành công. Vui lòng chờ quản lý duyệt.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nộp đơn: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}