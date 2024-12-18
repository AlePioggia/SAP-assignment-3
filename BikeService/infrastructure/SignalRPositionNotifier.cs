using BikeService.application;
using BikeService.controller;
using Microsoft.AspNetCore.SignalR;

namespace BikeService.infrastructure
{
    public class SignalRPositionNotifier : IPositionNotifier
    {
        private readonly IHubContext<BikeHub> _hubContext;

        public SignalRPositionNotifier(IHubContext<BikeHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyPositionAsync(string bikeId, int x, int y)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveBikePosition", bikeId, x, y);
        }

        public async Task NotifySimulationStoppedAsync(string bikeId)
        {
            await _hubContext.Clients.All.SendAsync("SimulationStopped", bikeId);
        }
    }
}
