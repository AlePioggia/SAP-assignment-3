using RentalService.domain.entities;

namespace RentalService.application.ride
{
    public interface IRideRepository
    {
        Task AddRideAsync(Ride ride);
        Task<Ride> GetRideByIdAsync(string rideId);
        Task<IEnumerable<Ride>> GetActiveRidesByUserIdAsync(string userId);
        Task UpdateRideAsync(Ride ride);
        Task EndRideAsync(string rideId);
    }
}
