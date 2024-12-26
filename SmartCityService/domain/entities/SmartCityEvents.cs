namespace SmartCityService.domain.entities
{
    public record InitializeBikesEvent(List<Bike> Bikes);
    public record InitializeStationsEvent(List<Station> Stations);  
    public record BikeUpdatedEvent(string BikeId, int X, int Y);
    public record ChargeBikeRequest(string BikeId, string Station);
    public record ChargeBikeResponse(string BikeId, string Station);
}
