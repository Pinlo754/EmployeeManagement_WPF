using Models.Entities;
using System.Collections.Generic;
using System.Windows;

namespace EmployeeManagement
{
    public partial class NotificationsWindow : Window
    {
        public NotificationsWindow(List<Notification> notifications)
        {
            InitializeComponent();

            NotificationsListView.ItemsSource = notifications;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}