namespace BikeService.domain.entities
{
    public record BikeCreatedEvent(string BikeId, string Model, string Status, int X, int Y);
    public record BikePositionUpdatedEvent(string BikeId, int X, int Y);
    public record BikeStatusChangedEvent(string BikeId, string Status);

    public class BikePositionUpdatedConfirmationEvent
    {
        public string BikeId { get; set; }
        public bool Success { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
