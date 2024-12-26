using static RentalService.domain.events.RideEvents;

namespace RentalService.application
{
    public interface IEventPublisher
    {
        Task PublishAsync(BikePositionUpdatedEvent positionEvent);
        Task PublishAsync(RideEndedEvent rideEndedEvent);
        Task PublishAsync(ChargeEBikeEvent chargeEBikeEvent);
        Task PublishAsync(RequestStationInfoEvent requestStationInfoEvent);
        Task PublishAsync(RequestAllStationsEvent requestAllStationsEvent);
        Task PublishAsync(ReachUserEvent reachUser);
        Task WaitForPositionUpdateConfirmation(string bikeId);
    }
}
