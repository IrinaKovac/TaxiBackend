using Models.Auth;
using Models.Ride;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesLogic.Implementations
{
    internal class RideLogic : Contracts.Logic.IRideLogic
    {
        private Contracts.Database.IData dbService;
        public RideLogic(Contracts.Database.IData dbService) 
        {
            this.dbService = dbService;
        }

        public async Task<Ride> CreateRide(CreateRideRequest request, string clientEmail)
        {
            var now = DateTime.UtcNow;
            now = DateTime.SpecifyKind(now, DateTimeKind.Utc);
            var unixTimestamp = new DateTimeOffset(now).ToUnixTimeMilliseconds();

            var newRide = new Models.Ride.Ride()
            {
                ClientEmail = clientEmail,
                CreatedAtTimestamp = unixTimestamp,
                DriverEmail = null,
                EndAddress = request.EndAddress,
                StartAddress = request.StartAddress,
                Price = request.Price,
                Status = RideStatus.CREATED,
                EstimatedDriverArrival = now.AddSeconds(request.EstimatedDriverArrivalSeconds),
                EstimatedRideEnd = null
            };

            return await dbService.CreateRide(newRide);
        }

        public Task<EstimateRideResponse> EstimateRide(EstimateRideRequest request)
        {
            var randomGen = new Random();

            return Task.FromResult(
                new EstimateRideResponse()
                {
                    PriceEstimate = randomGen.NextSingle() * 1000,
                    EstimatedDriverArrivalSeconds = randomGen.Next(60) + 60 // Max 1 hour
                });
        }

        public async Task<IEnumerable<Ride>> GetAllRides()
        {
            return await dbService.GetRides(default);
        }

        public async Task<IEnumerable<Ride>> GetNewRides()
        {
            return await dbService.GetRides(new QueryRideParams()
            {
                Status = RideStatus.CREATED
            });
        }

        public async Task<Ride> GetRideStatus(string clientEmail, long rideCreatedAtTimestamp)
        {
            return await dbService.GetRide(clientEmail, rideCreatedAtTimestamp);
        }

        public async Task<IEnumerable<Ride>> GetUsersRides(string userEmail, UserType userType)
        {
            switch (userType)
            {
                case UserType.CLIENT:
                    {
                        var clientRides = new List<Ride>();
                        foreach (RideStatus status in Enum.GetValues(typeof(RideStatus)))
                        {
                            var rides = await dbService.GetRides(new QueryRideParams()
                            {
                                ClientEmail = userEmail,
                                Status = RideStatus.COMPLETED
                            });
                            clientRides.AddRange(rides);
                        }
                        return clientRides;
                    }
                case UserType.DRIVER:
                    return await dbService.GetRides(new QueryRideParams()
                    {
                        DriverEmail = userEmail,
                        Status = RideStatus.COMPLETED
                    });
                case UserType.ADMIN:
                default:
                    return await GetAllRides();
            }
        }

        public async Task<Ride> UpdateRide(UpdateRideRequest request, string driverEmail)
        {
            // Driver accepted the ride
            if (request.Status == RideStatus.ACCEPTED)
            {
                var randomGen = new Random();

                var rideWithTimeEstimate = new Models.Ride.UpdateRideWithTimeEstimate()
                {
                    ClientEmail = request.ClientEmail,
                    RideCreatedAtTimestamp = request.RideCreatedAtTimestamp,
                    Status = request.Status,
                    RideEstimateSeconds = randomGen.Next(60)
                };

                return await dbService.UpdateRide(rideWithTimeEstimate, driverEmail);
            }

            return await dbService.UpdateRide(request, driverEmail);
        }
    }
}
