namespace BikeService.application
{
    public interface IBikeService
    {
        Task CreateBike(Guid bikeId, string model, int X, int Y);
        Task UpdateBikePosition(Guid bikeId, int X, int Y);
    }
}
