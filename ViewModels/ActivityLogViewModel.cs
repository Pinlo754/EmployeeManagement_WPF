using Models.Entities;
using Models.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ViewModels
{
    public class ActivityLogViewModel : INotifyPropertyChanged
    {
        private readonly ActivityLogRepository _logRepo;

        public ObservableCollection<ActivityLog> Logs { get; set; } = new ObservableCollection<ActivityLog>();
        public ObservableCollection<Account> Accounts { get; set; } = new ObservableCollection<Account>();

        private Account? _selectedAccount;
        public Account? SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                if (_selectedAccount != value)
                {
                    _selectedAccount = value;
                    OnPropertyChanged();
                    LoadLogs();
                }
            }
        }

        public ActivityLogViewModel(ActivityLogRepository logRepo, EmployeeRepository empRepo)
        {
            _logRepo = logRepo;

            // Tạo danh sách Account kèm tùy chọn "Tất cả"
            var allAccounts = empRepo.GetAll()
                .Select(e => e.Account)
                .Where(a => a != null)
                .Distinct()
                .ToList();

            Accounts.Clear();

            // Thêm tùy chọn "Tất cả" (AccountId = null)
            Accounts.Add(new Account { AccountId = 0, Username = "[Tất cả tài khoản]" });

            foreach (var acc in allAccounts)
                Accounts.Add(acc!);

            // Chọn mặc định là "Tất cả"
            SelectedAccount = Accounts.First();
        }

        public void LoadLogs()
        {
            Logs.Clear();

            if (SelectedAccount == null || SelectedAccount.AccountId == 0)
            {
                // Hiển thị tất cả log
                var allLogs = _logRepo.GetAll();
                foreach (var log in allLogs)
                    Logs.Add(log);
            }
            else
            {
                // Hiển thị log theo tài khoản
                var logs = _logRepo.GetByAccount(SelectedAccount.AccountId);
                foreach (var log in logs)
                    Logs.Add(log);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
