using SmartCityService.domain.entities;
using System.Collections.Concurrent;

namespace SmartCityService.application
{
    public class BikeRepository : IBikeRepository
    {
        private readonly ConcurrentDictionary<string, Bike> _bikes = new();

        public void AddOrUpdate(Bike bike)
        {
            _bikes[bike.Id] = bike;
        }

        public void AddOrUpdateBulk(IEnumerable<Bike> bikes)
        {
            foreach (var bike in bikes)
            {
                _bikes[bike.Id] = bike;
            }
        }

        public IEnumerable<Bike> GetAll()
        {
            return _bikes.Values;
        }

        public Bike? GetById(string id)
        {
            _bikes.TryGetValue(id, out var bike);
            return bike;
        }
    }
}
