using Models.Entities;
using Models.Repositories;
using System.Windows;
using System.Windows.Controls;
using ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement
{
    public partial class Login : Window
    {
        private readonly LoginViewModel _viewModel;

        public Login()
        {
            InitializeComponent();

            var accountRepo = new AccountRepository(new EmployeeManagementContext());
            _viewModel = new LoginViewModel(accountRepo);

            this.DataContext = _viewModel;

            // Subscribe event để hiển thị message
            _viewModel.ShowMessage += msg => MessageBox.Show(msg);
            _viewModel.LoginSuccess += account =>
            {
                Window nextWindow;
                if (account.Role == "Admin")
                    nextWindow = new AdminWindow(account);
                else
                    nextWindow = new EmployeeWindow(account);

                nextWindow.Show();
                this.Close();
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
