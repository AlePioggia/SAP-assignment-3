using static RentalService.domain.events.RideEvents;

namespace RentalService.application
{
    public interface IEventPublisher
    {
        Task PublishAsync(BikePositionUpdatedEvent positionEvent);
        Task PublishAsync(RideEndedEvent rideEndedEvent);
    }
}
