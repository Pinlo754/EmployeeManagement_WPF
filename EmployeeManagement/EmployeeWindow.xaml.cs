using System.Windows;
using Models.Entities;

namespace EmployeeManagement
{
    public partial class EmployeeWindow : Window
    {
        private readonly Account _account;

        public EmployeeWindow(Account account)
        {
            InitializeComponent();
            _account = account;

            // Hiển thị tên nhân viên
            WelcomeText.Text = $"Xin chào, {_account.FullName ?? _account.Username}!";
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
        }
    }
}
