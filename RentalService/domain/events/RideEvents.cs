
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

        public class ReachUserEvent
        {
            public string BikeId { get; }
            public int X { get; }
            public int Y { get; }

            public ReachUserEvent(string bikeId, int x, int y)
            {
                BikeId = bikeId;
                X = x;
                Y = y;
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

        public class ChargeEBikeEvent
        {
            public string BikeId { get; }
            public (int, int) Position { get; }
            public ChargeEBikeEvent(string bikeId, int x, int y)
            {
                BikeId = bikeId;
                Position = (x, y);
            }
        }   

        public class RequestStationInfoEvent
        {
            public string StationId { get; }

            public RequestStationInfoEvent(string stationId)
            {
                StationId = stationId;
            }
        }

        public class  RequestAllStationsEvent
        {
            public RequestAllStationsEvent()
            {
            }
        }

        public class RequestNearestStationEvent
        {
            public int X { get; }
            public int Y { get; }

            public RequestNearestStationEvent(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public class BikeReachedUserEvent
        {
            public string BikeId { get; }
            public int X { get; }
            public int Y { get; }

            public BikeReachedUserEvent(string bikeId, int x, int y)
            {
                BikeId = bikeId;
                X = x;
                Y = y;
            }
        }

        public class BikePositionUpdatedConfirmationEvent
        {
            public string BikeId { get; }
            public int X { get; }
            public int Y { get; }

            public BikePositionUpdatedConfirmationEvent(string bikeId, int x, int y)
            {
                BikeId = bikeId;
                X = x;
                Y = y;
            }
        }
    }
}
