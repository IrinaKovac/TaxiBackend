using Microsoft.ServiceFabric.Services.Remoting;
using Models.Auth;
using Models.UserTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Database
{
    [ServiceContract]
    public interface IDriverDataService : IService
    {
        [OperationContract]
        Task<bool> CreateDriver(Driver appModel);

        [OperationContract]
        Task<DriverStatus> GetDriverStatus(string driverEmail);

        [OperationContract]
        Task<bool> UpdateDriverStatus(string driverEmail, DriverStatus status);

        [OperationContract]
        Task<Models.UserTypes.Driver> UpdateDriverProfile(UpdateUserProfileRequest request, string partitionKey, string rowKey);

        [OperationContract]
        Task<IEnumerable<Driver>> ListAllDrivers();
    }
}
