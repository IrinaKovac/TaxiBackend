using Contracts.Database;
using Contracts.Logic;
using Models.UserTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesLogic.Implementations
{
    internal class DriverLogic : IDriverLogic
    {
        private IData dbService;

        public DriverLogic(IData dbService) 
        {
            this.dbService = dbService;
        }

        public async Task<DriverStatus> GetDriverStatus(string driverEmail)
        {
            return await dbService.GetDriverStatus(driverEmail);
        }

        public async Task<IEnumerable<Driver>> ListAllDrivers()
        {
            return await dbService.ListAllDrivers();
        }

        public async Task<bool> UpdateDriverStatus(string driverEmail, DriverStatus status)
        {
            return await dbService.UpdateDriverStatus(driverEmail, status);
        }
    }
}
