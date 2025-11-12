using BatterySwapStationManagement.Repositories.PhongNT.DTO;
using BatterySwapStationManagement.Repositories.PhongNT.ModelExtensions;
using BatterySwapStationManagement.Repositories.PhongNT.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatterySwapStationManagement.Services.PhongNT
{
    public interface IBatteryPhongNTServices
    {
        Task<int> CreateAsync(BatteryPhongNT battery);

        Task<int> UpdateAsync(BatteryPhongNT battery);

        Task<List<BatteryPhongNT>> GetAllAsync();

        Task<BatteryPhongNT> GetByIdAsync(Guid id);

        Task<List<BatteryPhongNT>> SearchAsync(Station station, bool? status, string address);

        Task<PaginationResult<List<BatteryPhongNT>>> SearchWithPagingAsync(BatterySearchRequest request);

        Task<bool> RemoveAsync(Guid id);
    }
}
