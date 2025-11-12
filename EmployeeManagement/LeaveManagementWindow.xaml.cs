using Models.Entities;
using Models.Repositories;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ViewModels; 

namespace EmployeeManagement
{
    public partial class LeaveManagementWindow : Window
    {
        private readonly LeaveRepository _leaveRepo;
        private const int TOTAL_ALLOWED_LEAVE = 10;

        public LeaveManagementWindow()
        {
            InitializeComponent();
            _leaveRepo = new LeaveRepository(new EmployeeManagementContext());
            this.Loaded += LeaveManagementWindow_Loaded;
        }

        private async void LeaveManagementWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadLeaveRequests();  
            await LoadEmployeeSummary(); 
        }

        private async Task LoadLeaveRequests()
        {
            var requests = await _leaveRepo.GetAllAsync();
            LeaveDataGrid.ItemsSource = requests;
        }

        private async Task LoadEmployeeSummary()
        {
            var summary = await _leaveRepo.GetEmployeeLeaveSummaryAsync(TOTAL_ALLOWED_LEAVE);
            EmployeeLeaveSummaryGrid.ItemsSource = summary;
        }

        private async void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedLeaf = LeaveDataGrid.SelectedItem as Leaf;
            if (selectedLeaf == null)
            {
                MessageBox.Show("Vui lòng chọn một đơn (từ Bảng 1) để duyệt.", "Chưa chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedLeaf.Status == "Approved")
            {
                MessageBox.Show("Đơn này đã được duyệt từ trước.", "Thông tin", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int daysTaken = await _leaveRepo.GetApprovedLeaveDaysTakenAsync((int)selectedLeaf.EmployeeId);
            int daysRequesting = selectedLeaf.DaysCount ?? 0;

            if ((daysTaken + daysRequesting) > TOTAL_ALLOWED_LEAVE)
            {
                var result = MessageBox.Show($"Nhân viên này chỉ còn {TOTAL_ALLOWED_LEAVE - daysTaken} ngày phép.\nĐơn này xin {daysRequesting} ngày sẽ VƯỢT QUÁ SỐ NGÀY PHÉP.\n\nBạn vẫn muốn duyệt?",
                                 "Cảnh báo: Vượt quá ngày phép", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            selectedLeaf.Status = "Approved";

            await _leaveRepo.UpdateAsync(selectedLeaf);
            MessageBox.Show($"Đã duyệt đơn nghỉ phép cho {selectedLeaf.Employee.FullName}.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);

            await LoadLeaveRequests();
            await LoadEmployeeSummary();
        }

        private async void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedLeaf = LeaveDataGrid.SelectedItem as Leaf;
            if (selectedLeaf == null)
            {
                MessageBox.Show("Vui lòng chọn một đơn (từ Bảng 1) để từ chối.", "Chưa chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedLeaf.Status == "Rejected")
            {
                MessageBox.Show("Đơn này đã bị từ chối từ trước.", "Thông tin", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            selectedLeaf.Status = "Rejected";

            await _leaveRepo.UpdateAsync(selectedLeaf);
            MessageBox.Show($"Đã từ chối đơn nghỉ phép của {selectedLeaf.Employee.FullName}.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);

            await LoadLeaveRequests();
            await LoadEmployeeSummary();
        }
    }
}