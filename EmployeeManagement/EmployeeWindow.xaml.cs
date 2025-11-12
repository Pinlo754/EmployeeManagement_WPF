using Models.Entities;
using Models.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EmployeeManagement
{
    public partial class EmployeeWindow : Window
    {
        private readonly Account _account;
        private readonly TimesheetRepository _timesheetRepo;
        private readonly NotificationRepository _notificationRepo;
        private Employee? _employee;

        public EmployeeWindow(Account account)
        {
            InitializeComponent();
            _account = account;
            WelcomeText.Text = $"Xin chào, {_account.FullName ?? _account.Username}!";
            var context = new EmployeeManagementContext();
            _timesheetRepo = new TimesheetRepository(context);
            _notificationRepo = new NotificationRepository(context);

            this.Loaded += EmployeeWindow_Loaded;
        }

        private async void EmployeeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _employee = _account.Employees.FirstOrDefault();
            if (_employee == null)
            {
                MessageBox.Show("Không tìm thấy hồ sơ nhân viên. Chức năng chấm công sẽ bị vô hiệu hóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                CheckInButton.IsEnabled = false;
                CheckOutButton.IsEnabled = false;
                return;
            }

            await UpdateCheckInUI();
        }

        private async Task UpdateCheckInUI()
        {
            if (_employee == null) return;

            var todayTimesheet = await _timesheetRepo.GetTodayTimesheetAsync(_employee.EmployeeId);

            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            var checkInDeadline = new TimeOnly(16, 0, 0); 

            if (todayTimesheet == null)
            {
                if (currentTime > checkInDeadline)
                {
                    CheckInButton.IsEnabled = false;
                    CheckOutButton.IsEnabled = false;
                    TimekeepingExpander.Header = "Chấm công (Đã qua giờ làm việc)";
                }
                else
                {
                    // Vẫn trong giờ Check In
                    CheckInButton.IsEnabled = true;
                    CheckOutButton.IsEnabled = false;
                    TimekeepingExpander.Header = "Chấm công"; 
                }
            }
            else if (todayTimesheet.CheckOut == null)
            {
                CheckInButton.IsEnabled = false;
                CheckOutButton.IsEnabled = true;
                TimekeepingExpander.Header = "Chấm công (Chờ Check Out)";
            }
            else
            {
                CheckInButton.IsEnabled = false;
                CheckOutButton.IsEnabled = false;
                TimekeepingExpander.Header = "Chấm công (Đã hoàn thành)";
            }
        }

        private async void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null) return;

            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            var currentTime = TimeOnly.FromDateTime(now);

            var checkInDeadline = new TimeOnly(16, 0, 0); 
            if (currentTime > checkInDeadline)
            {
                MessageBox.Show("Đã quá 16:00, bạn không thể Check In.", "Hết giờ Check In", MessageBoxButton.OK, MessageBoxImage.Warning);
                await UpdateCheckInUI(); 
                return;
            }

            try
            {
                var newTimesheet = new Timesheet
                {
                    EmployeeId = _employee.EmployeeId,
                    WorkDate = today,
                    CheckIn = currentTime,
                    Status = "Present"
                };

                await _timesheetRepo.CreateCheckInAsync(newTimesheet);
                MessageBox.Show($"Check In thành công!\nThời gian: {currentTime:HH:mm:ss}", "Chấm công", MessageBoxButton.OK, MessageBoxImage.Information);

                await UpdateCheckInUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi Check In: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null) return;

            var now = DateTime.Now;
            var currentTime = TimeOnly.FromDateTime(now);
            var overtimeThreshold = new TimeOnly(16, 0, 0);

            try
            {
                var todayTimesheet = await _timesheetRepo.GetTodayTimesheetAsync(_employee.EmployeeId);
                if (todayTimesheet == null || todayTimesheet.CheckIn == null)
                {
                    MessageBox.Show("Lỗi: Không tìm thấy bản ghi Check In để Check Out.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                todayTimesheet.CheckOut = currentTime;

                TimeSpan workedSpan = (TimeOnly)todayTimesheet.CheckOut - (TimeOnly)todayTimesheet.CheckIn;
                todayTimesheet.HoursWorked = (decimal)workedSpan.TotalHours;

                const decimal standardHours = 8.0m;
                if (todayTimesheet.HoursWorked >= standardHours)
                {
                    todayTimesheet.Status = "Approved";
                }
                else
                {
                    todayTimesheet.Status = "Check_Out sớm"; 
                }

                if (currentTime > overtimeThreshold)
                {
                    TimeOnly startTimeForOt = (TimeOnly)todayTimesheet.CheckIn > overtimeThreshold
                        ? (TimeOnly)todayTimesheet.CheckIn
                        : overtimeThreshold;

                    TimeSpan overtimeSpan = currentTime - startTimeForOt;
                    todayTimesheet.OvertimeHours = (decimal)overtimeSpan.TotalHours;
                }
                else
                {
                    todayTimesheet.OvertimeHours = 0;
                }

                await _timesheetRepo.UpdateCheckOutAsync(todayTimesheet);

                string otMessage = (todayTimesheet.OvertimeHours > 0)
                    ? $"\nGiờ Overtime: {todayTimesheet.OvertimeHours:F2} giờ"
                    : "";

                string statusMessage = $"\nTrạng thái: {todayTimesheet.Status}"; 

                MessageBox.Show($"Check Out thành công!\nThời gian: {currentTime:HH:mm:ss}" + otMessage + statusMessage, "Chấm công", MessageBoxButton.OK, MessageBoxImage.Information);

                await UpdateCheckInUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi Check Out: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
        }

        private void ViewProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_employee != null)
            {
                EmployeeInfoWindow infoWindow = new EmployeeInfoWindow(_account, _employee);
                infoWindow.Owner = this;
                infoWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Không tìm thấy hồ sơ nhân viên chi tiết cho tài khoản này.", "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ViewNotifications_Click(object sender, RoutedEventArgs e) // THÊM 'async'
        {
            if (_employee == null)
            {
                MessageBox.Show("Không tìm thấy hồ sơ nhân viên để tải thông báo.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                int? departmentId = _employee.DepartmentId;

                var notifications = await _notificationRepo.GetNotificationsForEmployeeAsync(departmentId);

                if (notifications.Count == 0)
                {
                    MessageBox.Show("Không có thông báo nào cho bạn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                NotificationsWindow notificationWindow = new NotificationsWindow(notifications);
                notificationWindow.Owner = this;
                notificationWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông báo: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyForLeave_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null)
            {
                MessageBox.Show("Không tìm thấy hồ sơ nhân viên để nộp đơn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Mở form mới và truyền thông tin nhân viên vào
            LeaveRequestFormWindow form = new LeaveRequestFormWindow(_employee);
            form.Owner = this;
            form.ShowDialog();
        }
    }
}