using Models.Entities;
using Models.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace ViewModels
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        private readonly EmployeeRepository _employeeRepo;

        public ObservableCollection<DepartmentReport> DeptReports { get; set; } = new ObservableCollection<DepartmentReport>();
        public ObservableCollection<PositionReport> PositionReports { get; set; } = new ObservableCollection<PositionReport>();
        public ObservableCollection<GenderReport> GenderReports { get; set; } = new ObservableCollection<GenderReport>();

        public ReportViewModel(EmployeeRepository employeeRepo)
        {
            _employeeRepo = employeeRepo;
            LoadReports();
        }

        public void LoadReports()
        {
            var employees = _employeeRepo.GetAll();

            // Thống kê theo phòng ban
            DeptReports.Clear();
            foreach (var group in employees.GroupBy(e => e.Department?.DepartmentName ?? "Chưa phân công"))
            {
                DeptReports.Add(new DepartmentReport { DepartmentName = group.Key, Count = group.Count() });
            }

            // Thống kê theo chức vụ
            PositionReports.Clear();
            foreach (var group in employees.GroupBy(e => e.Position ?? "Chưa có"))
            {
                PositionReports.Add(new PositionReport { Position = group.Key, Count = group.Count() });
            }

            // Thống kê theo giới tính
            GenderReports.Clear();
            foreach (var group in employees.GroupBy(e => e.Gender ?? "Chưa xác định"))
            {
                GenderReports.Add(new GenderReport { Gender = group.Key, Count = group.Count() });
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class DepartmentReport { public string DepartmentName { get; set; } = ""; public int Count { get; set; } }
    public class PositionReport { public string Position { get; set; } = ""; public int Count { get; set; } }
    public class GenderReport { public string Gender { get; set; } = ""; public int Count { get; set; } }
}
