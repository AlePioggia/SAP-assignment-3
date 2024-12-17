using RentalService.domain.entities;
using static RentalService.domain.events.RideEvents;

namespace RentalService.application.ride.simulation
{
    public class RideSimulation
    {
        private readonly Ride _ride;
        private int _creditIntervalMs = 1000;
        private CancellationTokenSource _cts;
        private readonly IPositionNotifier _positionNotifier;
        private readonly IEventPublisher _eventPublisher;

        public RideSimulation(Ride ride, IPositionNotifier positionNotifier, IEventPublisher eventPublisher)
        {
            _ride = ride;
            _cts = new CancellationTokenSource();
            _positionNotifier = positionNotifier;
            _eventPublisher = eventPublisher;
        }

        public async Task StartSimulationAsync(int credit)
        {
            _ride.StartTime = DateTime.UtcNow;

            for (int i = 0; i < 10; i++)
            {
                if (_cts.Token.IsCancellationRequested) break;

                var positionEvent = new BikePositionUpdatedEvent(_ride.EBikeId, 1, 1, DateTime.UtcNow);
                await _positionNotifier.NotifyPositionAsync(_ride.EBikeId, 1, 1);
                await _eventPublisher.PublishAsync(positionEvent);

                _ride.DeductCredit(1);
                await Task.Delay(_creditIntervalMs, _cts.Token);
            }

            await _positionNotifier.NotifySimulationStoppedAsync(_ride.EBikeId);
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
