using System.Windows;
using ViewModels;

namespace EmployeeManagement
{
    public partial class PayrollManagementWindow : Window
    {
        public PayrollManagementWindow()
        {
            InitializeComponent();
            DataContext = new PayrollViewModel(); // ViewModel tự tạo Repository
        }
    }
}
