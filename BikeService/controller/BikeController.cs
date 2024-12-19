using BikeService.application;
using Microsoft.AspNetCore.Mvc;

namespace BikeService.controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikeController : ControllerBase
    {
        private readonly IBikeService _bikeService;

        public BikeController(IBikeService bikeService)
        {
            _bikeService = bikeService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBike([FromBody] CreateBikeRequest request)
        {
            await _bikeService.CreateBike(request.BikeId, request.Model, request.X, request.Y);
            return Ok("Bike created!");
        }

        [HttpPost("update-position")]
        public async Task<IActionResult> UpdatePosition(string bikeId, int X, int Y)
        {
            await _bikeService.UpdateBikePosition(bikeId, X, Y);
            return Ok("Bike position updated");
        }
    }

    public class CreateBikeRequest
    {
        public string BikeId { get; set; }
        public string Model { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
