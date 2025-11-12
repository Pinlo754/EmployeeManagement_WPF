using Models.Entities;
using Models.Repositories;
using System.Collections.Generic;
using System.Windows;

namespace EmployeeManagement
{
    public partial class NotificationManagementWindow : Window
    {
        private readonly Account _adminAccount;
        private readonly NotificationRepository _notificationRepo;
        private readonly EmployeeManagementContext _context;

        public NotificationManagementWindow(Account adminAccount)
        {
            InitializeComponent();
            _adminAccount = adminAccount;

            _context = new EmployeeManagementContext();
            _notificationRepo = new NotificationRepository(_context);

            this.Loaded += ManagementWindow_Loaded;
        }

        private async void ManagementWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadNotifications();
        }

        private async Task LoadNotifications()
        {
            var notifications = await _notificationRepo.GetAllAdminAsync();
            NotificationsListView.ItemsSource = notifications;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var form = new NotificationFormWindow(_adminAccount, null);
            bool? dialogResult = form.ShowDialog();

            if (dialogResult == true)
            {
                var newNotification = form.GetNotification();
                await _notificationRepo.AddAsync(newNotification);
                await LoadNotifications();
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedNotification = NotificationsListView.SelectedItem as Notification;
            if (selectedNotification == null)
            {
                MessageBox.Show("Vui lòng chọn một thông báo để sửa.", "Chưa chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var form = new NotificationFormWindow(_adminAccount, selectedNotification);
            bool? dialogResult = form.ShowDialog();

            if (dialogResult == true)
            {
                var updatedNotification = form.GetNotification();
                await _notificationRepo.UpdateAsync(updatedNotification);
                await LoadNotifications(); 
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedNotification = NotificationsListView.SelectedItem as Notification;
            if (selectedNotification == null)
            {
                MessageBox.Show("Vui lòng chọn một thông báo để xóa.", "Chưa chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc muốn xóa thông báo '{selectedNotification.Title}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _notificationRepo.DeleteAsync(selectedNotification);
                await LoadNotifications(); 
            }
        }
    }
}