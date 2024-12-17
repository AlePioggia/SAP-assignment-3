using RentalService.application.ride;
using RentalService.domain.entities;
using MongoDB.Driver;

namespace RentalService.infrastructure.ride
{
    public class RideRepository : IRideRepository
    {
        private readonly IMongoCollection<MongoRide> _ridesCollection;

        public RideRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("RidesDB");
            _ridesCollection = database.GetCollection<MongoRide>("Rides");
        }

        public async Task AddRideAsync(Ride ride)
        {
            var mongoRide = MongoRide.FromDomainEntity(ride);
            await _ridesCollection.InsertOneAsync(mongoRide);
        }

        public async Task EndRideAsync(string rideId)
        {
            var update = Builders<MongoRide>.Update.Set(r => r.EndTime, DateTime.UtcNow);
            await _ridesCollection.UpdateOneAsync(r => r.Id == rideId, update);
        }

        public async Task<IEnumerable<Ride>> GetActiveRidesByUserIdAsync(string userId)
        {
            var mongoRides = await _ridesCollection.Find(r => r.UserId == userId && r.EndTime == null).ToListAsync();
            return mongoRides.Select(r => r.ToDomainEntity());
        }

        public async Task<Ride> GetRideByIdAsync(string rideId)
        {
            var mongoRide = await _ridesCollection.Find(r => r.Id == rideId).FirstOrDefaultAsync();
            return mongoRide?.ToDomainEntity();
        }

        public async Task UpdateRideAsync(Ride ride)
        {
            var mongoRide = MongoRide.FromDomainEntity(ride);
            await _ridesCollection.ReplaceOneAsync(r => r.Id == ride.Id, mongoRide);
        }
    }
}
