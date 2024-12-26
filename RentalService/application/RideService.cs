using RentalService.application.ride.simulation;
using RentalService.domain.entities;
using System.Collections.Concurrent;

namespace RentalService.application.ride
{
    public class RideService : IRideService
    {
        private readonly IRideRepository _rideRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ConcurrentDictionary<string, RideSimulation> _activeSimulations = new();

        public RideService(IRideRepository rideRepository, IEventPublisher eventPublisher)
        {
            _rideRepository = rideRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<string> StartRide(string userId, string eBikeId, (int, int) userPosition, int credit)
        {
            var ride = new Ride(userId, eBikeId);
            await _rideRepository.AddRideAsync(ride);

            var simulation = new RideSimulation(ride, userPosition,_eventPublisher);
            _activeSimulations[ride.Id] = simulation;
            _ = simulation.StartSimulationAsync(credit);

            return ride.Id;
        }

        public async Task EndRide(string rideId)
        {
            if (!_activeSimulations.TryRemove(rideId, out var simulation))
            {
                throw new InvalidOperationException("Simulation not found or already ended");
            }

            simulation.StopSimulation();
            var ride = simulation.GetRide();
            await _rideRepository.UpdateRideAsync(ride);
        }

        public async Task<Ride> GetRide(string rideId)
        {
            return await _rideRepository.GetRideByIdAsync(rideId);
        }
    }
}
