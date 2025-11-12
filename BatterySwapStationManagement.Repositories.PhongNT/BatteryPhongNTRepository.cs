using BatterySwapStationManagement.Repositories.PhongNT.Basic;
using BatterySwapStationManagement.Repositories.PhongNT.DTO;
using BatterySwapStationManagement.Repositories.PhongNT.ModelExtensions;
using BatterySwapStationManagement.Repositories.PhongNT.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BatterySwapStationManagement.Repositories.PhongNT
{
    public class BatteryPhongNTRepository : GenericRepository<BatteryPhongNT>
    {
        public BatteryPhongNTRepository()
        {

        }
        public BatteryPhongNTRepository(FA25_PRN232_SE1707_G5_BatterySwapStationManagement context) => _context = context;

        public async Task<List<BatteryPhongNT>> GetAllBatteriesAsync()
        {
            var item = await _context.BatteriesPhongNT.Include(x => x.BatteryHealthRecords).ToListAsync();

            var stationDTO = item.Select(u => new BatteryPhongNT
            {
                BatteryPhongNTId = u.BatteryPhongNTId,
                SerialNo = u.SerialNo,
                Model = u.Model,
                Station = u.Station,
                BatteryHealthRecords = u.BatteryHealthRecords,
                Capacity = u.Capacity,
                SoHcurrent = u.SoHcurrent,
                StationId = u.StationId,
                Status = u.Status,
                SwapTransactionNewBatteries = u.SwapTransactionNewBatteries,
                SwapTransactionOldBatteries = u.SwapTransactionOldBatteries,
            }).ToList();

            return stationDTO ?? new List<BatteryPhongNT>();
        }

        public async Task<BatteryPhongNT> GetByIdAsync(Guid id)
        {
            var item = await _context.BatteriesPhongNT.Include(x => x.BatteryHealthRecords).FirstOrDefaultAsync(x => x.BatteryPhongNTId == id);
            return item ?? new BatteryPhongNT();
        }

        public async Task<List<BatteryPhongNT>> SearchAsync(Station station, bool? status, string address)
        {
            var query = _context.BatteriesPhongNT
                .Include(b => b.BatteryHealthRecords)
                .Include(b => b.Station)
                .AsQueryable();

            if (station != null)
            {
                query = query.Where(b => b.StationId == station.StationId);
            }

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(address))
            {
                query = query.Where(b => b.Station.Address.Contains(address));
            }

            var result = await query
                .OrderByDescending(b => b.Station.CreatedAt)
                .ToListAsync();

            return result ?? new List<BatteryPhongNT>();
        }


        public async Task<PaginationResult<List<BatteryPhongNT>>> SearchPaginationAsync(
    Station station,
    bool? status,
    string address,
    int? currentPage,
    int? pageSize)
        {
            var items = await SearchAsync(station, status, address);
            var totalItems = items.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / (pageSize ?? 10));

            var pagedItems = items
                .Skip(((currentPage ?? 1) - 1) * (pageSize ?? 10))
                .Take(pageSize ?? 10)
                .ToList();

            return new PaginationResult<List<BatteryPhongNT>>
            {
                TotalItem = totalItems,
                TotalPage = totalPages,
                PageSize = pageSize ?? 10,
                CurrentPage = currentPage ?? 1,
                item = pagedItems
            };
        }


        public async Task<PaginationResult<List<BatteryPhongNT>>> GetAllPaginationAsync(int? currentPage, int? pageSize)
        {
            var query = _context.BatteriesPhongNT
                .Include(b => b.BatteryHealthRecords)
                .Include(b => b.Station)
                .OrderByDescending(b => b.Station.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / (pageSize ?? 10));

            var items = await query
                .Skip(((currentPage ?? 1) - 1) * (pageSize ?? 10))
                .Take(pageSize ?? 10)
                .ToListAsync();

            return new PaginationResult<List<BatteryPhongNT>>
            {
                TotalItem = totalItems,
                TotalPage = totalPages,
                PageSize = pageSize ?? 10,
                CurrentPage = currentPage ?? 1,
                item = items
            };
        }


    }
}
