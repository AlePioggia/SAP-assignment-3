namespace BikeService.application
{
    public interface IPositionNotifier
    {
        Task NotifyPositionAsync(string bikeId, int x, int y);
        Task NotifySimulationStoppedAsync(string bikeId);
    }
}
