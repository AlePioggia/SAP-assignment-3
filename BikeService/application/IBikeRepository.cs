namespace BikeService.application
{
    public interface IBikeRepository
    {
        Task SaveAsync(String streamName, IEnumerable<object> events);
        Task<IEnumerable<object>> LoadAsync(String streamName);
    }
}
