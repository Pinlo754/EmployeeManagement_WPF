using BatterySwapStationManagement.Repositories.PhongNT;
using BatterySwapStationManagement.Repositories.PhongNT.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatterySwapStationManagement.Services.PhongNT
{
    public class BatteryHealthRecordPhongNTService : IBatteryHealthRecordPhongNTServices
    {
        private readonly BatteryHealthRecordPhongNTRepository _repository;

        public BatteryHealthRecordPhongNTService()
        {
            _repository ??= new BatteryHealthRecordPhongNTRepository();
        }

        public async Task<List<BatteryHealthRecordPhongNT>> GetAllAsync()
        {
            try
            {
                var result = await _repository.GetAllAsync();
                return result;
            }
            catch (Exception ex)
            {
                return new List<BatteryHealthRecordPhongNT>();
            }
        }

        public async Task<BatteryHealthRecordPhongNT> GetByIdAsync(Guid id)
        {
            try
            {
                var result = await _repository.GetByIdAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                return new BatteryHealthRecordPhongNT();
            }
        }

        public async Task<int> CreateAsync(BatteryHealthRecordPhongNT record)
        {
            try
            {
                return await _repository.CreateAsync(record);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> UpdateAsync(BatteryHealthRecordPhongNT record)
        {
            try
            {
                var result = await _repository.UpdateAsync(record);
                if (result != null)
                {
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            try
            {
                var item = await _repository.GetByIdAsync(id);
                if (item != null)
                {
                    return await _repository.RemoveAsync(item);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<BatteryHealthRecordPhongNT>> GetByBatteryIdAsync(Guid batteryId)
        {
            try
            {
                var result = await _repository.GetByBatteryIdAsync(batteryId);
                return result;
            }
            catch (Exception ex)
            {
                return new List<BatteryHealthRecordPhongNT>();
            }
        }
    }
}
