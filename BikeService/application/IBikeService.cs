namespace BikeService.application
{
    public interface IBikeService
    {
        Task CreateBike(string bikeId, string model, int X, int Y);
        Task UpdateBikePosition(string bikeId, int X, int Y);
    }
}
