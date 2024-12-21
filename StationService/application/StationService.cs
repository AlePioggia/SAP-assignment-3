using Microsoft.AspNetCore.Http.HttpResults;
using StationService.domain.entities;

namespace StationService.application
{
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepository;

        public StationService(IStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
        }

        public async Task<string> chargeBike(string bikeId, string stationId)
        {
            Station station = await _stationRepository.FindById(stationId);
            station.Bikes.Add(bikeId);

            await _stationRepository.Update(station);

            return bikeId;
        }

        public async Task<string> CreateStationAsync((int, int) position, int capacity)
        {
            Station station = new Station(position, capacity, DateTime.UtcNow);
            await _stationRepository.Add(station);
            return station.Id;
        }

        public async Task<string> DeleteStationAsync(string id)
        {
            await _stationRepository.Delete(id);
            return id;
        }

        public async Task<Station> GetNearestStation(int x, int y)
        {
            var stations = await _stationRepository.FindAll();

            Station nearestStation = null;
            double nearestDistance = double.MaxValue;

            foreach (var station in stations)
            {
                double distance = Math.Sqrt(Math.Pow(station.Position.Item1 - x, 2) + Math.Pow(station.Position.Item2 - y, 2));
                if (distance < nearestDistance && station.Capacity > station.Bikes.Count)
                {
                    nearestDistance = distance;
                    nearestStation = station;
                }
            }

            return nearestStation;
        }

        public async Task<Station> GetStationByIdAsync(string id)
        {
            return await _stationRepository.FindById(id);
        }

        public async Task<IEnumerable<Station>> GetStationsAsync()
        {
            return await _stationRepository.FindAll();
        }
    }
}
