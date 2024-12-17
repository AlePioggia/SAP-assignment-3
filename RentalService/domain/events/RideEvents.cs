
namespace RentalService.domain.events
{
    public class RideEvents
    {
        public class BikePositionUpdatedEvent
        {
            public string BikeId { get; }
            public int X { get; }
            public int Y { get; }
            public DateTime Timestamp { get; }

            public BikePositionUpdatedEvent(string bikeId, int x, int y, DateTime timestamp)
            {
                BikeId = bikeId;
                X = x;
                Y = y;
                Timestamp = timestamp;
            }
        }

        public class RideEndedEvent
        {
            public string RideId { get; }
            public string BikeId { get; }
            public DateTime EndTime { get; }

            public RideEndedEvent(string rideId, string bikeId, DateTime endTime)
            {
                RideId = rideId;
                BikeId = bikeId;
                EndTime = endTime;
            }
        }
    }
}
