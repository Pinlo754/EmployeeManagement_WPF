using BatterySwapStationManagement.Repositories.PhongNT.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatterySwapStationManagement.Services.PhongNT
{
    public interface IBatteryHealthRecordPhongNTServices
    {
        Task<List<BatteryHealthRecordPhongNT>> GetAllAsync();

        Task<BatteryHealthRecordPhongNT> GetByIdAsync(Guid id);

        Task<List<BatteryHealthRecordPhongNT>> GetByBatteryIdAsync(Guid batteryId);
    }
}
