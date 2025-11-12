using Models.Entities;
using Models.Repositories;
using System.Linq;
using System.Windows;

namespace EmployeeManagement
{
    public partial class MonthlyReportWindow : Window
    {
        private readonly ReportRepository _reportRepo;

        public MonthlyReportWindow()
        {
            InitializeComponent();
            _reportRepo = new ReportRepository(new EmployeeManagementContext());
            this.Loaded += ReportWindow_Loaded;
        }

        private void ReportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MonthComboBox.ItemsSource = Enumerable.Range(1, 12).ToList();

            int currentYear = System.DateTime.Now.Year;
            YearComboBox.ItemsSource = Enumerable.Range(currentYear - 5, 6).ToList(); 

            MonthComboBox.SelectedItem = System.DateTime.Now.AddMonths(-1).Month;
            YearComboBox.SelectedItem = System.DateTime.Now.AddMonths(-1).Year;
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (MonthComboBox.SelectedItem == null || YearComboBox.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn tháng và năm.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int month = (int)MonthComboBox.SelectedItem;
            int year = (int)YearComboBox.SelectedItem;

            try
            {
                var reportData = await _reportRepo.GetMonthlyReportAsync(month, year);

                ReportDataGrid.ItemsSource = reportData;

                if (reportData.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy dữ liệu chấm công cho tháng/năm đã chọn.", "Không có dữ liệu", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tạo báo cáo: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}