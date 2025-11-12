using System.Windows;

namespace EmployeeManagement
{
    public partial class DepartmentFormWindow : Window
    {
        public string DepartmentName { get; set; } = string.Empty;
        public string? DepartmentDescription { get; set; }

        public DepartmentFormWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên phòng ban không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DepartmentName = txtName.Text.Trim();
            DepartmentDescription = txtDescription.Text.Trim();
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        protected override void OnContentRendered(System.EventArgs e)
        {
            base.OnContentRendered(e);
            txtName.Text = DepartmentName;
            txtDescription.Text = DepartmentDescription;
            txtName.Focus();
        }
    }
}
