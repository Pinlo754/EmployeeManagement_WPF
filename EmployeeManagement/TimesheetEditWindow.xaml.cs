using Models.Entities;
using System;
using System.Collections.Generic;
using System.Windows;

namespace EmployeeManagement
{
    public partial class TimesheetEditWindow : Window
    {
        private Timesheet _timesheet;

        public TimesheetEditWindow(Timesheet timesheet)
        {
            InitializeComponent();
            _timesheet = timesheet;
            this.Loaded += EditWindow_Loaded;
        }

        private void EditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EmployeeNameText.Text = _timesheet.Employee?.FullName ?? "Không rõ";
            WorkDateText.Text = _timesheet.WorkDate?.ToString("dd/MM/yyyy") ?? "N/A";

            CheckInTextBox.Text = _timesheet.CheckIn?.ToString("HH\\:mm\\:ss"); 
            CheckOutTextBox.Text = _timesheet.CheckOut?.ToString("HH\\:mm\\:ss");

            StatusComboBox.ItemsSource = new List<string> { "Present", "Approved", "Pending", "Check_Out sớm", "Absent" };
            StatusComboBox.SelectedItem = _timesheet.Status;

            RecalculateHours();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            TimeOnly newCheckIn, newCheckOut;

            if (!TimeOnly.TryParse(CheckInTextBox.Text, out newCheckIn))
            {
                MessageBox.Show("Giờ Check In không hợp lệ. Vui lòng dùng định dạng HH:mm:ss.", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeOnly.TryParse(CheckOutTextBox.Text, out newCheckOut))
            {
                MessageBox.Show("Giờ Check Out không hợp lệ. Vui lòng dùng định dạng HH:mm:ss.", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newCheckOut < newCheckIn)
            {
                MessageBox.Show("Giờ Check Out không thể sớm hơn Check In.", "Lỗi logic", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _timesheet.CheckIn = newCheckIn;
            _timesheet.CheckOut = newCheckOut;
            _timesheet.Status = StatusComboBox.SelectedItem as string;

            RecalculateHours();
            _timesheet.HoursWorked = decimal.Parse(HoursWorkedText.Text);
            _timesheet.OvertimeHours = decimal.Parse(OvertimeHoursText.Text);

            this.DialogResult = true;
            this.Close();
        }

        private void RecalculateHours()
        {
            if (TimeOnly.TryParse(CheckInTextBox.Text, out TimeOnly checkIn) &&
                TimeOnly.TryParse(CheckOutTextBox.Text, out TimeOnly checkOut) &&
                checkOut >= checkIn)
            {
                TimeSpan workedSpan = checkOut - checkIn;
                decimal totalHours = (decimal)workedSpan.TotalHours;
                HoursWorkedText.Text = totalHours.ToString("N2");

                var overtimeThreshold = new TimeOnly(16, 0, 0);
                decimal overtimeHours = 0;
                if (checkOut > overtimeThreshold)
                {
                    TimeOnly startTimeForOt = checkIn > overtimeThreshold ? checkIn : overtimeThreshold;
                    TimeSpan overtimeSpan = checkOut - startTimeForOt;
                    overtimeHours = (decimal)overtimeSpan.TotalHours;
                }
                OvertimeHoursText.Text = overtimeHours.ToString("N2");
            }
            else
            {
                HoursWorkedText.Text = "0.00";
                OvertimeHoursText.Text = "0.00";
            }
        }

        public Timesheet GetUpdatedTimesheet()
        {
            return _timesheet;
        }
    }
}