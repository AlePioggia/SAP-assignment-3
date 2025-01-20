using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StationService.application;
using StationService.domain.entities;

namespace StationService.controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly IStationService _stationService;

        public StationController(IStationService stationService)
        {
            _stationService = stationService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetStations()
        {
            var stations = await _stationService.GetStationsAsync();
            return Ok(stations);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetStationById(string id)
        {
            var station = await _stationService.GetStationByIdAsync(id);
            return Ok(station);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateStation(CreateStationRequest station)
        {
            await _stationService.CreateStationAsync((station.X, station.Y), station.Capacity);
            return Ok(station);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<string> DeleteStation(string id)
        {
            return await _stationService.DeleteStationAsync(id);
        }

        [Authorize]
        [HttpGet("nearest")]
        public async Task<IActionResult> GetNearestStation(int x, int y)
        {
            var station = await _stationService.GetNearestStation(x, y);
            return Ok(station);
        }
    }

    public class CreateStationRequest
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Capacity { get; set; }
    }
}
