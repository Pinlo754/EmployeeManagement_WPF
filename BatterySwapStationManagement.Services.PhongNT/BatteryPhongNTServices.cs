using BatterySwapStationManagement.Repositories.PhongNT;
using BatterySwapStationManagement.Repositories.PhongNT.DTO;
using BatterySwapStationManagement.Repositories.PhongNT.ModelExtensions;
using BatterySwapStationManagement.Repositories.PhongNT.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatterySwapStationManagement.Services.PhongNT
{
    public class BatteryPhongNTServices : IBatteryPhongNTServices
    {
        private readonly BatteryPhongNTRepository _repository;

        public BatteryPhongNTServices() => _repository ??= new BatteryPhongNTRepository();

        public Task<int> CreateAsync(BatteryPhongNT battery)
        {
            return _repository.CreateAsync(battery);
        }

        public async Task<int> UpdateAsync(BatteryPhongNT battery)
        {
            try
            {
                var result = await _repository.UpdateAsync(battery);
                if (result != null)
                {
                    return 1;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<List<BatteryPhongNT>> GetAllAsync()
        {
            try
            {
                var items = await _repository.GetAllAsync();
                return items;
            }
            catch (Exception)
            {
                return new List<BatteryPhongNT>();
            }
        }

        public async Task<BatteryPhongNT> GetByIdAsync(Guid id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception)
            {
                return new BatteryPhongNT();
            }
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            try
            {
                var item = await _repository.GetByIdAsync(id);
                if (item != null)
                {
                    var result = await _repository.RemoveAsync(item);
                    if (result)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<BatteryPhongNT>> SearchAsync(Station station, bool? status, string address)
        {
            try
            {
                var result = await _repository.SearchAsync(station, status, address);
                return result;
            }
            catch (Exception)
            {
                return new List<BatteryPhongNT>();
            }
        }

        public async Task<PaginationResult<List<BatteryPhongNT>>> SearchWithPagingAsync(BatterySearchRequest request)
        {
            try
            {
                var item = await _repository.SearchPaginationAsync(request.Station, request.Status, request.Address, request.CurrentPage, request.PageSize);
                return item;
            }
            catch (Exception)
            {
                return new PaginationResult<List<BatteryPhongNT>>()
                {
                    item = new List<BatteryPhongNT>(),
                    CurrentPage = 0,
                    PageSize = 0,
                    TotalItem = 0,
                    TotalPage = 0
                };
            }
        }
    }
}
