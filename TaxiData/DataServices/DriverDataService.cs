using AzureInterface;
using AzureInterface.DTO;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Models.Auth;
using Models.UserTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiData.DataImplementations;

namespace TaxiData.DataServices
{
    internal class DriverDataService : BaseDataService<Models.UserTypes.Driver, AzureInterface.Entities.Driver>, Contracts.Database.IDriverDataService
    {
        public DriverDataService(
            AzureTableCRUD<AzureInterface.Entities.Driver> storageWrapper, 
            IDTOConverter<AzureInterface.Entities.Driver, Models.UserTypes.Driver> converter, 
            Synchronizer<AzureInterface.Entities.Driver, Models.UserTypes.Driver> synchronizer,
            IReliableStateManager stateManager
        ) : base(storageWrapper, converter, synchronizer, stateManager)
        {}


        public async Task<DriverStatus> GetDriverStatus(string driverEmail)
        {
            var dict = await GetReliableDictionary();
            using var txWrapper = new StateManagerTransactionWrapper(stateManager.CreateTransaction());

            var existingDriver = await dict.TryGetValueAsync(txWrapper.transaction, $"{UserType.DRIVER}{driverEmail}");

            if (!existingDriver.HasValue)
            {
                return default;
            }

            return existingDriver.Value.Status;
        }

        public async Task<bool> UpdateDriverStatus(string driverEmail, DriverStatus status)
        {
            var dict = await GetReliableDictionary();
            using var txWrapper = new StateManagerTransactionWrapper(stateManager.CreateTransaction());

            var existingDriver = await dict.TryGetValueAsync(txWrapper.transaction, $"{UserType.DRIVER}{driverEmail}");
            if (!existingDriver.HasValue)
            {
                return false;
            }
            existingDriver.Value.Status = status;
            var result = await dict.TryUpdateAsync(txWrapper.transaction, $"{UserType.DRIVER}{driverEmail}", existingDriver.Value, existingDriver.Value);
            return result;
        }

        public async Task<Driver> UpdateDriverProfile(UpdateUserProfileRequest request, string partitionKey, string rowKey)
        {
            var dict = await GetReliableDictionary();
            using var txWrapper = new StateManagerTransactionWrapper(stateManager.CreateTransaction());
            var key = $"{partitionKey}{rowKey}";
            var existing = await dict.TryGetValueAsync(txWrapper.transaction, key);

            if (!existing.HasValue)
            {
                return null;
            }

            if (request.Password != null)
            {
                existing.Value.Password = request.Password;
            }

            if (request.Username != null)
            {
                existing.Value.Username = request.Username;
            }

            if (request.Address != null)
            {
                existing.Value.Address = request.Address;
            }

            if (request.ImagePath != null)
            {
                existing.Value.ImagePath = request.ImagePath;
            }

            if (request.Fullname != null)
            {
                existing.Value.Fullname = request.Fullname;
            }

            if (!string.IsNullOrEmpty(request.DateOfBirth))
            {
                DateTime dateOfBirth;
                if (DateTime.TryParse(request.DateOfBirth, out dateOfBirth))
                {
                    existing.Value.DateOfBirth = dateOfBirth;
                }
            }

            var updated = await dict.TryUpdateAsync(txWrapper.transaction, key, existing.Value, existing.Value);

            return updated ? existing.Value : null;
        }

        public async Task<IEnumerable<Models.UserTypes.Driver>> ListAllDrivers()
        {
            var dict = await GetReliableDictionary();
            using var txWrapper = new StateManagerTransactionWrapper(stateManager.CreateTransaction());

            var collectionEnum = await dict.CreateEnumerableAsync(txWrapper.transaction);
            var asyncEnum = collectionEnum.GetAsyncEnumerator();

            var drivers = new List<Models.UserTypes.Driver>();

            while (await asyncEnum.MoveNextAsync(default))
            {
                var driverEntity = asyncEnum.Current.Value;
                if (driverEntity != null)
                {
                    drivers.Add(driverEntity);
                }
            }

            return drivers;
        }


        public async Task<bool> Create(Models.UserTypes.Driver driver)
        {
            var dict = await GetReliableDictionary();
            using var txWrapper = new StateManagerTransactionWrapper(stateManager.CreateTransaction());
            var dictKey = $"{driver.Type}{driver.Email}";
            var created = await dict.AddOrUpdateAsync(txWrapper.transaction, dictKey, driver, (key, value) => value);
            return created != null;
        }

        public async Task<bool> CreateDriver(Driver appModel)
        {
            return await Create(appModel);
        }
    }
}
