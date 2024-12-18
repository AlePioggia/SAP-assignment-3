namespace BikeService.domain.entities
{
    public record BikeCreatedEvent(string BikeId, string Model, string Status, int X, int Y);
    public record BikePositionUpdatedEvent(string BikeId, int X, int Y);
    public record BikeStatusChangedEvent(string BikeId, string Status);
}
