using BatterySwapStationManagement.Repositories.PhongNT.Basic;
using BatterySwapStationManagement.Repositories.PhongNT.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatterySwapStationManagement.Repositories.PhongNT
{
    public class BatteryHealthRecordPhongNTRepository : GenericRepository<BatteryHealthRecordPhongNT>
    {
        public BatteryHealthRecordPhongNTRepository()
        {
        }

        public BatteryHealthRecordPhongNTRepository(FA25_PRN232_SE1707_G5_BatterySwapStationManagement context)
        {
            _context = context;
        }

        public async Task<List<BatteryHealthRecordPhongNT>> GetAllAsync()
        {
            return await _context.BatteryHealthRecordsPhongNT
                .Include(x => x.Battery)
                .ToListAsync();
        }

        public async Task<BatteryHealthRecordPhongNT> GetByIdAsync(Guid id)
        {
            var record = await _context.BatteryHealthRecordsPhongNT
                .Include(x => x.Battery)
                .FirstOrDefaultAsync(x => x.RecordPhongNTId == id);

            return record ?? new BatteryHealthRecordPhongNT();
        }

        public async Task<List<BatteryHealthRecordPhongNT>> GetByBatteryIdAsync(Guid batteryId)
        {
            return await _context.BatteryHealthRecordsPhongNT
                .Where(x => x.BatteryPhongNTId == batteryId)
                .Include(x => x.Battery)
                .OrderByDescending(x => x.MeasuredAt)
                .ToListAsync();
        }
    }
}
