using BatterySwapStationManagement.Repositories.PhongNT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatterySwapStationManagement.Services.PhongNT
{
    public interface ISystemUserAccountServices
    {
        public Task<SystemUserAccount> GetUserAccount(string userName , string password);
    }
}
