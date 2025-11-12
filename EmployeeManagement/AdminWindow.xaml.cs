using System.Windows;
using Models.Entities;

namespace EmployeeManagement
{
    public partial class AdminWindow : Window
    {
        private readonly Account _account;

        public AdminWindow(Account account)
        {
            InitializeComponent();
            _account = account;

            // Hiển thị tên Admin
            WelcomeText.Text = $"Xin chào, {_account.FullName ?? _account.Username}!";
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
        }

        private void ManageEmployees_Click(object sender, RoutedEventArgs e)
        {
            var employeeWindow = new EmployeeManagementWindow();
            employeeWindow.Show();
        }

        private void ManageDepartments_Click(object sender, RoutedEventArgs e)
        {
            var departmentWindow = new DepartmentManagementWindow();
            departmentWindow.Show();
        }
        private void ManagePayrolls_Click(object sender, RoutedEventArgs e)
        {
            var payrollWindow = new PayrollManagementWindow();
            payrollWindow.Show();
        }
        private void ManageNotifications_Click(object sender, RoutedEventArgs e)
        {
            var managementWindow = new NotificationManagementWindow(_account);
            managementWindow.ShowDialog();
        }
        private void ManageTimesheets_Click(object sender, RoutedEventArgs e)
        {
            var managementWindow = new TimesheetManagementWindow();
            managementWindow.ShowDialog();
        }

        private void ManageLeave_Click(object sender, RoutedEventArgs e)
        {
            var managementWindow = new LeaveManagementWindow();
            managementWindow.ShowDialog();
        }
    }
}
