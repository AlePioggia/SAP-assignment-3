
using Microsoft.AspNetCore.SignalR;

namespace RentalService.controller
{
    public class BikeHub : Hub
    {
        public async Task BroadcastBikePosition(int bikeId, int x, int y)
        {
            await Clients.All.SendAsync("ReceiveBikePosition", bikeId, x, y);
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client disconnected {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
