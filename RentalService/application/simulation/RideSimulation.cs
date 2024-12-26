using RentalService.domain.entities;
using static RentalService.domain.events.RideEvents;

namespace RentalService.application.ride.simulation
{
    public class RideSimulation
    {
        private readonly Ride _ride;
        private int _creditIntervalMs = 1000;
        private CancellationTokenSource _cts;
        private (int, int) _startingPosition;
        private readonly IEventPublisher _eventPublisher;

        public RideSimulation(Ride ride, (int, int) startingPosition, IEventPublisher eventPublisher)
        {
            _ride = ride;
            _cts = new CancellationTokenSource();
            _eventPublisher = eventPublisher;
            _startingPosition = startingPosition;
        }

        public async Task StartSimulationAsync(int credit)
        {
            var reachUser = new ReachUserEvent(_ride.EBikeId, _startingPosition.Item1, _startingPosition.Item2);
            await _eventPublisher.PublishAsync(reachUser);

            _ride.StartTime = DateTime.UtcNow;

            for (int i = 0; i < 10; i++)
            {
                if (_cts.Token.IsCancellationRequested) break;

                var positionEvent = new BikePositionUpdatedEvent(_ride.EBikeId, 1, 1, DateTime.UtcNow);
                await _eventPublisher.PublishAsync(positionEvent);
                await Task.Delay(_creditIntervalMs, _cts.Token);
            }

            var chargeEbike = new ChargeEBikeEvent(_ride.EBikeId, 1, 1);
            await _eventPublisher.PublishAsync(chargeEbike);
        }

        public void StopSimulation()
        {
            _ride.EndRide();
            _cts.Cancel();
        }

        public Ride GetRide()
        {
            return _ride;
        }
    }
}
