
using BikeService.domain.entities;

namespace BikeService.application
{
    public class BikeService : IBikeService
    {
        private readonly IBikeRepository _repository;

        public BikeService(IBikeRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateBike(string bikeId, string model, int X, int Y)
        {
            var bike = new Bike(bikeId, model, X, Y);
            await _repository.SaveAsync($"bike-{bikeId}", bike.Changes);
        }

        public async Task<(int X, int Y)> GetBikeCurrentPosition(string bikeId)
        {
            var history = await _repository.LoadAsync($"bike-{bikeId}");
            var bike = new Bike(bikeId, "", 0, 0);
            bike.Load(history);

            return (bike.X, bike.Y);
        }

        public async Task UpdateBikePosition(string bikeId, int X, int Y)
        {
            var history = await _repository.LoadAsync($"bike-{bikeId}");
            var bike = new Bike(bikeId, "", 0, 0);
            bike.Load(history);

            bike.UpdatePosition(X, Y);
            await _repository.SaveAsync($"bike-{bikeId}", bike.Changes);
        }
    }
}
