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
        public async Task<IActionResult> CreateBike(string bikeId, string model, int X, int Y)
        {
            await _bikeService.CreateBike(bikeId, model, X, Y);
            return Ok("Bike created!");
        }

        [HttpPost("update-position")]
        public async Task<IActionResult> UpdatePosition(string bikeId, int X, int Y)
        {
            await _bikeService.UpdateBikePosition(bikeId, X, Y);
            return Ok("Bike position updated");
        }
    }
}
