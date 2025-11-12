using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels 
{
    public class MonthlyReportView
    {
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? DepartmentName { get; set; }
        public int TotalWorkDays { get; set; } // Tổng ngày công
        public int TotalLeaveDays { get; set; } // Tổng ngày nghỉ (đã duyệt)
        public decimal TotalOTHours { get; set; } // Tổng giờ OT
    }
}
