using System;

namespace BatterySwapStationManagement.Repositories.PhongNT.DTO
{
    public class BatteryPhongNTDTO
    {
        public Guid BatteryPhongNTId { get; set; }
        public string SerialNo { get; set; }
        public string Model { get; set; }
        public decimal Capacity { get; set; }
        public decimal SoHcurrent { get; set; }
        public Guid StationId { get; set; }
        public bool Status { get; set; }

        public string StationName { get; set; }
        public string Address { get; set; }
    }
}
