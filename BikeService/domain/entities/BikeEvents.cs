namespace BikeService.domain.entities
{
    public record BikeCreatedEvent(Guid BikeId, string Model, string Status, int X, int Y);
    public record BikePositionUpdatedEvent(Guid BikeId, int X, int Y);
    public record BikeStatusChangedEvent(Guid BikeId, string Status);
}
