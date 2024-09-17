using AzureInterface;
using AzureInterface.DTO;
using AzureInterface.Entities;
using Contracts.Database;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiData.DataImplementations;

namespace TaxiData.DataServices
{
    internal class AuthDataService : BaseDataService<Models.Auth.UserProfile, AzureInterface.Entities.User>, Contracts.Database.IAuthDataService
    {

        // 1. Dodaj polje za DriverDataService instancu
        private readonly DriverDataService _driverDataService;

        public AuthDataService(
            AzureTableCRUD<User> storageWrapper,
            IDTOConverter<User, UserProfile> converter,
            Synchronizer<User, UserProfile> synchronizer,
            IReliableStateManager stateManager,
            DriverDataService driverDataService
        )
            : base(storageWrapper, converter, synchronizer, stateManager)
        {
            _driverDataService = driverDataService;
        }

        public async Task<UserProfile> UpdateUserProfile(UpdateUserProfileRequest request, string partitionKey, string rowKey)
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

            // 4. Koristi instancu _driverDataService za pozivanje metode UpdateDriverProfile
            if (partitionKey == UserType.DRIVER.ToString() && updated)
            {
                // Ovde pozivamo metodu koja ažurira Driver tabelu
                await _driverDataService.UpdateDriverProfile(request, partitionKey, rowKey);
            }

            return updated ? existing.Value : null;
        }
    
        public async Task<UserProfile> GetUserProfile(string partitionKey, string rowKey)
        {
            var dict = await GetReliableDictionary();
            using var txWrapper = new StateManagerTransactionWrapper(stateManager.CreateTransaction());
            var existing = await dict.TryGetValueAsync(txWrapper.transaction, $"{partitionKey}{rowKey}");
            return existing.Value;
        }

        public async Task<bool> Exists(string partitionKey, string rowKey)
        {
            var userProfile = await GetUserProfile(partitionKey, rowKey);
            return userProfile != null;
        }

        public async Task<bool> ExistsWithPwd(string partitionKey, string rowKey, string password)
        {
            var dict = await GetReliableDictionary();
            using var txWrapper = new StateManagerTransactionWrapper(stateManager.CreateTransaction());
            var existing = await dict.TryGetValueAsync(txWrapper.transaction, $"{partitionKey}{rowKey}");
            if (existing.HasValue)
            {
                return existing.Value.Password.Equals(password) &&
                    existing.Value.Email.Equals(rowKey) &&
                    existing.Value.Type.ToString().Equals(partitionKey);
            }
            return false;
        }

        public async Task<bool> ExistsSocialMediaAuth(string partitionKey, string rowKey)
        {
            var dict = await GetReliableDictionary();
            using var txWrapper = new StateManagerTransactionWrapper(stateManager.CreateTransaction());
            var existing = await dict.TryGetValueAsync(txWrapper.transaction, $"{partitionKey}{rowKey}");
            if (existing.HasValue)
            {
                return existing.Value.Email.Equals(rowKey) &&
                    existing.Value.Type.ToString().Equals(partitionKey);
            }
            return false;
        }

        public async Task<bool> Create(UserProfile appModel)
        {
            var dict = await GetReliableDictionary();
            using var txWrapper = new StateManagerTransactionWrapper(stateManager.CreateTransaction());
            var dictKey = $"{appModel.Type}{appModel.Email}";
            var created = await dict.AddOrUpdateAsync(txWrapper.transaction, dictKey, appModel, (key, value) => value);
            return created != null;
        }

        public async Task<bool> CreateUser(UserProfile appModel)
        {
            return await Create(appModel);
        }
    }
}
