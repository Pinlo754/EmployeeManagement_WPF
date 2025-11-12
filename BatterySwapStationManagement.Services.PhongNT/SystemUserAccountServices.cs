using BatterySwapStationManagement.Repositories.PhongNT;
using BatterySwapStationManagement.Repositories.PhongNT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatterySwapStationManagement.Services.PhongNT
{
    public class SystemUserAccountServices : ISystemUserAccountServices
    {
        private readonly SystemUserAccountRepository _repository;

        public SystemUserAccountServices()  => _repository ??= new SystemUserAccountRepository();
   

        public async Task<SystemUserAccount> GetUserAccount(string userName, string password)
        {
            //return await _repository.GetUserAccount(userName, password);

            try
            {
                return await _repository.GetUserAccount(userName, password);
                
            }catch (Exception ex)
            {
                return null;
            }
        }
    }
}
