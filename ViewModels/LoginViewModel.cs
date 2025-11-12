using Models.Entities;
using Models.Repositories;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewModels.Helper;
namespace ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AccountRepository _accountRepo;

        public LoginViewModel(AccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
            LoginCommand = new RelayCommand(async _ => await LoginAsync());
        }

        private string _username = "";
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        // Event để hiển thị MessageBox
        public event Action<string>? ShowMessage;

        // Event khi đăng nhập thành công
        public event Action<Account>? LoginSuccess;

        private async Task LoginAsync()
        {
            var account = await _accountRepo.LoginAsync(Username, Password);
            if (account != null)
            {
                // Gọi event LoginSuccess
                LoginSuccess?.Invoke(account);
                Session.CurrentUser = account;
                // Thông báo login thành công
                ShowMessage?.Invoke($"Đăng nhập thành công với quyền {account.Role}");
            }
            else
            {
                ShowMessage?.Invoke("Tên đăng nhập hoặc mật khẩu sai!");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
