namespace StationService.domain.entities
{
    public class StationEvents
    {
        public record StationRequestedEvent(string Id);
        public record AllStationsRequestedEvent();
        public record ChargeEbike(string StationId, string BikeId);
        public record GetNearestStation(int X, int Y);
    }
}
