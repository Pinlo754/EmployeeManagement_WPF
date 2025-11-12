using BatterySwapStationManagement.Repositories.PhongNT.Models;

namespace BatterySwapStationManagement.Repositories.PhongNT.ModelExtensions
{
    public class BatterySearchRequest
    {
        public Station Station { get; set; }
        public bool? Status { get; set; }
        public string Address { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
    }
}
