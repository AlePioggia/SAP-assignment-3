using SmartCityService.domain.entities;
using System.Collections.Concurrent;

namespace SmartCityService.application
{
    public class StationRepository : IStationRepository
    {
        private ConcurrentDictionary<string, Station> _stations = new();

        public void AddOrUpdate(Station station)
        {
            _stations[station.Id] = station;
        }

        public void AddOrUpdateBulk(IEnumerable<Station> stations)
        {
            foreach (var station in stations)
            {
                _stations[station.Id] = station;
            }
        }

        public void ChargeBike(string bikeId, string stationId) 
        {
            var station = _stations[stationId];
            station.Bikes.Add(bikeId);
        }

        public IEnumerable<Station> GetAll()
        {
            return _stations.Values;
        }

        public Station? GetStationById(string id)
        {
            return _stations[id];
        }
    }
}
