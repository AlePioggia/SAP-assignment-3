using SmartCityService.domain.entities;

namespace SmartCityService.application
{
    public interface IStationRepository
    {
        public void AddOrUpdate(Station station);
        public void AddOrUpdateBulk(IEnumerable<Station> stations);
        public Station? GetStationById(string id);
        public IEnumerable<Station> GetAll();
        public void ChargeBike(string bikeId, string stationId);   
    }
}
