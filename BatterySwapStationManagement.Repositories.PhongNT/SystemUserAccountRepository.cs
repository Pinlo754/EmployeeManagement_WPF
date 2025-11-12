using BatterySwapStationManagement.Repositories.PhongNT.Basic;
using BatterySwapStationManagement.Repositories.PhongNT.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatterySwapStationManagement.Repositories.PhongNT
{
    public class SystemUserAccountRepository : GenericRepository<SystemUserAccount>
    {
        public SystemUserAccountRepository()
        {                
        }

        public SystemUserAccountRepository(FA25_PRN232_SE1707_G5_BatterySwapStationManagement context) => _context = context;    

        public async Task<SystemUserAccount> GetUserAccount(string userName , string password)
        {
            return await _context.SystemUserAccounts
                .FirstOrDefaultAsync(x => x.UserName == userName && x.Password == password && x.IsActive == true);
        }

    }
}
