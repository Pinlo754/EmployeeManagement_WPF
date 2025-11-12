using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{ 
    public class EmployeeLeaveSummaryView
    {
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int TotalAllowed { get; set; } 
        public int TotalTaken { get; set; } 
        public int RemainingDays { get; set; } 
    }
}
