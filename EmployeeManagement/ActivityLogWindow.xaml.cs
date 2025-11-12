using Models.Entities;
using Models.Repositories;
using System.Windows;
using ViewModels;

namespace EmployeeManagement
{
    public partial class ActivityLogWindow : Window
    {
        private readonly ActivityLogViewModel _viewModel;

        public ActivityLogWindow()
        {
            InitializeComponent();

            // Khởi tạo context và repository
            var context = new EmployeeManagementContext();
            var logRepo = new ActivityLogRepository(context);
            var empRepo = new EmployeeRepository(context);

            // Khởi tạo ViewModel
            _viewModel = new ActivityLogViewModel(logRepo, empRepo);

            // Gán DataContext
            DataContext = _viewModel;

            // Load log ngay khi mở cửa sổ
            _viewModel.LoadLogs();
        }
    }
}
