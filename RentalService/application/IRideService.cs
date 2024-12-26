using RentalService.domain.entities;

namespace RentalService.application.ride
{
    public interface IRideService
    {
        Task<string> StartRide(string userId, string eBikeId, (int, int) startingPosition, int credit);
        Task EndRide(string rideId);
        Task<Ride> GetRide(string rideId);
    }
}
