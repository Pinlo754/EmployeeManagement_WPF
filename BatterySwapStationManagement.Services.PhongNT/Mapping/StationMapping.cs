using BatterySwapStationManagement.Repositories.PhongNT.DTO;
using BatterySwapStationManagement.Repositories.PhongNT.Models;
using System;

namespace BatterySwapStationManagement.Services.PhongNT.Mapping
{
    public static class BatteryMapping
    {


        public static BatteryPhongNTDTO ToDto(this BatteryPhongNT b) => new BatteryPhongNTDTO
        {
            BatteryPhongNTId = b.BatteryPhongNTId,
            SerialNo = b.SerialNo,
            Model = b.Model,
            Capacity = b.Capacity,
            SoHcurrent = b.SoHcurrent,
            StationId = b.StationId,
            Status = b.Status,
            StationName = b.Station?.Name ?? string.Empty,
            Address = b.Station?.Address ?? string.Empty
        };
    }
}
