using RentalService.domain.entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace RentalService.infrastructure
{
    public class MongoRide
    {
        [BsonId]
        public string Id { get; set; }

        public string UserId { get; set; }
        public string EBikeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int CreditUsed { get; set; } = 0;

        public Ride ToDomainEntity()
        {
            return new Ride(UserId, EBikeId)
            {
                Id = Id,
                StartTime = StartTime,
                EndTime = EndTime,
                CreditUsed = CreditUsed
            };
        }

        public static MongoRide FromDomainEntity(Ride ride)
        {
            return new MongoRide
            {
                Id = ride.Id,
                UserId = ride.UserId,
                EBikeId = ride.EBikeId,
                StartTime = ride.StartTime,
                EndTime = ride.EndTime,
                CreditUsed = ride.CreditUsed
            };
        }
    }
}
