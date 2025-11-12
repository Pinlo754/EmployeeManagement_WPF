using Models.Entities;
using Models.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace EmployeeManagement
{
    public partial class NotificationFormWindow : Window
    {
        private readonly DepartmentRepository _deptRepo;
        private readonly Account _adminAccount;
        private Notification _notification; 
        private bool _isEditMode;

        private readonly Department _allDepartmentsOption = new Department
        {
            DepartmentId = 0,
            DepartmentName = "-- Gửi cho tất cả nhân viên --"
        };

        public NotificationFormWindow(Account adminAccount, Notification? notificationToEdit)
        {
            InitializeComponent();

            _adminAccount = adminAccount;
            _deptRepo = new DepartmentRepository(new EmployeeManagementContext());

            if (notificationToEdit == null)
            {
                _notification = new Notification();
                _isEditMode = false;
                Title = "Tạo thông báo mới";
            }
            else
            {
                _notification = notificationToEdit;
                _isEditMode = true;
                Title = "Chỉnh sửa thông báo";
                TitleTextBox.Text = _notification.Title;
                MessageTextBox.Text = _notification.Message;
            }

            this.Loaded += NotificationFormWindow_Loaded;
        }

        private async void NotificationFormWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var departments = await _deptRepo.GetAllAsync();

            departments.Insert(0, _allDepartmentsOption);

            DepartmentComboBox.ItemsSource = departments;

            if (_isEditMode && _notification.TargetDepartmentId != null)
            {
                DepartmentComboBox.SelectedValue = _notification.TargetDepartmentId;
            }
            else
            {
                DepartmentComboBox.SelectedItem = _allDepartmentsOption;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (Department)DepartmentComboBox.SelectedItem;

            if (selectedItem.DepartmentId == _allDepartmentsOption.DepartmentId)
            {
                _notification.TargetDepartmentId = null;
            }
            else
            {
                _notification.TargetDepartmentId = selectedItem.DepartmentId;
            }

            _notification.Title = TitleTextBox.Text;
            _notification.Message = MessageTextBox.Text;

            if (!_isEditMode)
            {
                _notification.CreatedBy = _adminAccount.AccountId;
                _notification.CreatedAt = DateTime.Now;
            }

            this.DialogResult = true;
            this.Close();
        }

        public Notification GetNotification()
        {
            return _notification;
        }
    }
}