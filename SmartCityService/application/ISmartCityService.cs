using SmartCityService.domain.entities;

namespace SmartCityService.application
{
    public interface ISmartCityService
    {
        public void InitializeBikes(IEnumerable<Bike> bikes);
        public void InitializeStations(IEnumerable<Station> stations);
        public Station? FindNearestStation(int bikeX, int bikeY);
        public void AddOrUpdateBike(Bike bike);
        public void AddOrUpdateStation(Station station);
        public Bike? GetBikeById(string id);
        public Station? GetStationById(string id);
    }
}
