using SmartCityService.domain.entities;

namespace SmartCityService.application
{
    public interface IBikeRepository
    {
        void AddOrUpdate(Bike bike);
        void AddOrUpdateBulk(IEnumerable<Bike> bikes);
        Bike? GetById(string id);
        IEnumerable<Bike> GetAll();
    }
}
