using AzureInterface.DTO;
using AzureInterface.Entities;
using Microsoft.ServiceFabric.Data;
using Models;
using Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiData.DataServices
{
    internal class DataServiceFactory
    {
        public AuthDataService AuthDataService { get; private set; }
        public DriverDataService DriverDataService { get; private set; }
        public RideDataService RideDataService { get; private set; }
        public RatingDataService DriverRatingDataService { get; private set; }

        public DataServiceFactory(
            IReliableStateManager stateManager,
            AzureInterface.AzureTableCRUD<AzureInterface.Entities.User> userStorageWrapper,
            AzureInterface.AzureTableCRUD<AzureInterface.Entities.Driver> driverStorageWrapper,
            AzureInterface.AzureTableCRUD<AzureInterface.Entities.Ride> rideStorageWrapper,
            AzureInterface.AzureTableCRUD<AzureInterface.Entities.RideRating> driverRatingStorageWrapper
        ) 
        {
            /*var userDto = new UserDTO();
            AuthDataService = new AuthDataService(
                userStorageWrapper,
                userDto,
                new DataImplementations.Synchronizer<User, Models.Auth.UserProfile>(
                    userStorageWrapper, 
                    typeof(UserProfile).Name, 
                    userDto, 
                    stateManager
                ),
                stateManager
            );
            var driverDto = new DriverDTO();
            DriverDataService = new DriverDataService(
                driverStorageWrapper,
                driverDto,
                new DataImplementations.Synchronizer<Driver, Models.UserTypes.Driver>(
                    driverStorageWrapper,
                    typeof(Driver).Name,
                    driverDto,
                    stateManager
                ),
                stateManager
            );*/

            // Kreiramo instancu DriverDataService prije AuthDataService
            var driverDto = new DriverDTO();
            DriverDataService = new DriverDataService(
                driverStorageWrapper,
                driverDto,
                new DataImplementations.Synchronizer<Driver, Models.UserTypes.Driver>(
                    driverStorageWrapper,
                    typeof(Driver).Name,
                    driverDto,
                    stateManager
                ),
                stateManager
            );

            // Prosljeđujemo instancu DriverDataService u konstruktor AuthDataService
            var userDto = new UserDTO();
            AuthDataService = new AuthDataService(
                userStorageWrapper,
                userDto,
                new DataImplementations.Synchronizer<User, Models.Auth.UserProfile>(
                    userStorageWrapper,
                    typeof(UserProfile).Name,
                    userDto,
                    stateManager
                ),
                stateManager,
                DriverDataService // Prosljeđivanje instance
            );

            var rideDto = new RideDTO();
            RideDataService = new RideDataService(
                rideStorageWrapper,
                rideDto,
                new DataImplementations.Synchronizer<Ride, Models.Ride.Ride>(
                    rideStorageWrapper,
                    typeof(Ride).Name,
                    rideDto,
                    stateManager
                ),
                stateManager
            );

            var driverRatingDto = new DriverRatingDTO();
            DriverRatingDataService = new RatingDataService(
                driverRatingStorageWrapper,
                driverRatingDto,
                new DataImplementations.Synchronizer<RideRating, Models.UserTypes.RideRating>(
                    driverRatingStorageWrapper,
                    typeof(RideRating).Name,
                    driverRatingDto,
                    stateManager
                ),
                stateManager
            );
        }

        public async Task SyncAzureTablesWithDict()
        {
            await AuthDataService.SyncAzureTablesWithDict();
            await DriverDataService.SyncAzureTablesWithDict();
            await RideDataService.SyncAzureTablesWithDict();
            await DriverRatingDataService.SyncAzureTablesWithDict();
        }
        public async Task SyncDictWithAzureTable()
        {
            await AuthDataService.SyncDictWithAzureTable();
            await DriverDataService.SyncDictWithAzureTable();
            await RideDataService.SyncDictWithAzureTable();
            await DriverRatingDataService.SyncDictWithAzureTable();
        }
    }
}
