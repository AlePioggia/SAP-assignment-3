using MongoDB.Bson.Serialization.Attributes;
using StationService.domain.entities;

namespace StationService.infrastructure
{
    public class MongoStation
    {
        [BsonId]
        public string Id { get; set; }

        public (int, int) Position { get; set; }
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }

        public Station toDomainStation()
        {
            return new Station(Position, Capacity, CreatedAt)
            {
                Id = Id
            };
        }

        public MongoStation FromDomainStation(Station station)
        {
            return new MongoStation
            {
                Id = station.Id,
                Position = station.Position,
                Capacity = station.Capacity,
                CreatedAt = station.CreatedAt
            };

        }
    }
}