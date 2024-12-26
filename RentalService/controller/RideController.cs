using RentalService.application.ride;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BikeManagementService.controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RideController : ControllerBase
    {
        private readonly IRideService _rideService;

        public RideController(IRideService rideService)
        {
            _rideService = rideService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartRide([FromBody] RideStartRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.EBikeId))
            {
                return BadRequest("Invalid user ID or eBike ID.");
            }

            var rideId = await _rideService.StartRide(request.UserId, request.EBikeId, request.UserPosition, 10);

            if (rideId == null)
            {
                return BadRequest("Unable to start ride. Please check your details.");
            }

            return Ok(rideId);
        }

        [HttpPost("end/{rideId}")]
        public async Task<IActionResult> EndRide(string rideId)
        {
            var ride = await _rideService.GetRide(rideId);

            if (ride == null || ride.EndTime.HasValue)
            {
                return NotFound("Ride not found or already ended.");
            }

            await _rideService.EndRide(rideId);

            return Ok(new { Message = "Ride ended successfully", RideId = rideId });
        }

        [HttpGet("{rideId}")]
        public async Task<IActionResult> GetRideById(string rideId)
        {
            var ride = await _rideService.GetRide(rideId);

            if (ride == null)
            {
                return NotFound("Ride not found.");
            }
            
            return Ok(ride);
        }
    }

    public class RideStartRequest
    {
        public string? UserId { get; set; }
        public string? EBikeId { get; set; }
        public (int, int) UserPosition { get; set; }
    }
}
