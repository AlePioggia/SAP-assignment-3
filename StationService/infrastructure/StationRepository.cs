using MongoDB.Driver;
using StationService.application;
using StationService.domain.entities;

namespace StationService.infrastructure
{
    public class StationRepository : IStationRepository
    {
        private readonly IMongoCollection<MongoStation> _stations;

        public StationRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("StationsDb");
            _stations = database.GetCollection<MongoStation>("stations");
        }


        public async Task<Station> FindById(string id)
        {
            var mongoStation = await _stations.Find(s => s.Id == id).FirstOrDefaultAsync();
            return mongoStation?.toDomainStation();
        }

        public async Task<IEnumerable<Station>> FindAll()
        {
            var mongoStations = await _stations.Find(s => true).ToListAsync();
            return mongoStations.ConvertAll(s => s.toDomainStation());
        }

        public async Task<Station> Add(Station station)
        {
            var mongoStation = new MongoStation
            {
                Id = Guid.NewGuid().ToString(),
                Position = station.Position,
                Capacity = station.Capacity,
                CreatedAt = station.CreatedAt
            };

            await _stations.InsertOneAsync(mongoStation);
            return mongoStation.toDomainStation();
        }

        public async Task<Station> Update(Station station)
        {
            var mongoStation = new MongoStation
            {
                Id = station.Id,
                Position = station.Position,
                Capacity = station.Capacity,
                CreatedAt = station.CreatedAt
            };

            var result = await _stations.ReplaceOneAsync(s => s.Id == station.Id, mongoStation);
            return result.IsAcknowledged ? mongoStation.toDomainStation() : null;
        }

        public async Task<Station> Delete(string id)
        {
            var mongoStation = await _stations.FindOneAndDeleteAsync(s => s.Id == id);
            return mongoStation?.toDomainStation();
        }
    }
}
