using Models.Entities;
using System;
using System.Windows;
using System.Windows.Media.Imaging; 


namespace EmployeeManagement
{
    public partial class EmployeeInfoWindow : Window
    {
        private const string NotAvailable = "N/A";

        public EmployeeInfoWindow(Account account, Employee employee)
        {
            InitializeComponent();
            LoadEmployeeData(account, employee);
        }

        private void LoadEmployeeData(Account account, Employee employee)
        {
            FullNameText.Text = employee.FullName ?? account.FullName ?? NotAvailable;
            UsernameText.Text = account.Username;
            RoleText.Text = account.Role ?? NotAvailable;
            PositionText.Text = employee.Position ?? NotAvailable;
            EmailText.Text = employee.Email ?? NotAvailable;
            PhoneText.Text = employee.Phone ?? NotAvailable;
            AddressText.Text = employee.Address ?? NotAvailable;
            DobText.Text = employee.DateOfBirth?.ToString("dd/MM/yyyy") ?? NotAvailable;
            StartDateText.Text = employee.StartDate?.ToString("dd/MM/yyyy") ?? NotAvailable;
            DepartmentText.Text = employee.Department?.DepartmentName ?? NotAvailable;
            if (!string.IsNullOrEmpty(employee.AvatarUrl))
            {
                try
                {
                    Uri uriSource = new Uri(employee.AvatarUrl, UriKind.Absolute);
                    AvatarImage.Source = new BitmapImage(uriSource);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể tải ảnh đại diện. Đảm bảo URL hợp lệ. Lỗi: {ex.Message}", "Lỗi tải ảnh", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ShowDefaultAvatar();
                }
            }
            else
            {
                ShowDefaultAvatar();
            }
        }

        private void ShowDefaultAvatar()
        {
            try
            {
                AvatarImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/default_avatar.png"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải ảnh mặc định: {ex.Message}");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}