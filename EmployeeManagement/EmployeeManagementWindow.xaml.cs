using Models.Entities;
using Models.Repositories;
using System.Windows;
using ViewModels;

namespace EmployeeManagement
{
    public partial class EmployeeManagementWindow : Window
    {
        private readonly EmployeeManagementViewModel _viewModel;

        public EmployeeManagementWindow()
        {
            InitializeComponent();

            var empRepo = new EmployeeRepository(new EmployeeManagementContext());
            var depRepo = new DepartmentRepository(new EmployeeManagementContext());

            _viewModel = new EmployeeManagementViewModel(empRepo, depRepo);

            DataContext = _viewModel;
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var form = new AddEmployeeWindow();
            if (form.ShowDialog() == true)
            {
                _viewModel.AddEmployee(form.Employee);
            }
        }

        private void DetailEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedEmployee != null)
            {
                var detailWindow = new EmployeeDetailWindow(_viewModel.SelectedEmployee, _viewModel);
                detailWindow.ShowDialog();
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearFilter();
        }
    }
}
