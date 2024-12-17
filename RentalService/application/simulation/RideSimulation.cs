using RentalService.domain.entities;

namespace RentalService.application.ride.simulation
{
    public class RideSimulation
    {
        private readonly Ride _ride;
        private int _creditIntervalMs = 1000;
        private CancellationTokenSource _cts;
        private readonly IPositionNotifier _positionNotifier;

        public RideSimulation(Ride ride, IPositionNotifier positionNotifier)
        {
            _ride = ride;
            _cts = new CancellationTokenSource();
            _positionNotifier = positionNotifier;
        }

        public async Task StartSimulationAsync(int credit)
        {
            //var eBike = await _eBikeService.GetEBike(_ride.EBikeId);
            //if (eBike == null)
            //{
            //    throw new InvalidOperationException("Bike not found");
            //}
            //var X = eBike.X;
            //var Y = eBike.Y;

            //_ride.StartTime = DateTime.Now;
            //for (int i = 0; i < credit; i++)
            //{
            //    X = X + 1;
            //    Y = Y + 1;
            //    await _eBikeService.UpdatePosition(_ride.EBikeId, X, Y);
            //    _ride.DeductCredit(1);

            //    await _positionNotifier.NotifyPositionAsync(_ride.EBikeId, X, Y);

            //    await Task.Delay(1000);
            //}

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
