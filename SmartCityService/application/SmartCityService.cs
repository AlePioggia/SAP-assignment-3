using SmartCityService.domain.entities;

namespace SmartCityService.application
{
    public class SmartCityService : ISmartCityService
    {
        private readonly IBikeRepository _bikeRepository;
        private readonly IStationRepository _stationRepository;

        public SmartCityService(IBikeRepository bikeRepository, IStationRepository stationRepository)
        {
            _bikeRepository = bikeRepository;
            _stationRepository = stationRepository;
        }

        public void InitializeBikes(IEnumerable<Bike> bikes)
        {
            _bikeRepository.AddOrUpdateBulk(bikes);
        }

        public void InitializeStations(IEnumerable<Station> stations)
        {
            _stationRepository.AddOrUpdateBulk(stations);
        }

        public Station? FindNearestStation(int bikeX, int bikeY)
        {
            var stations = _stationRepository.GetAll();

            Station? nearestStation = null;
            double nearestDistance = double.MaxValue;

            foreach (var station in stations)
            {
                var distance = CalculateDistance(bikeX, bikeY, station.Position.Item1, station.Position.Item2);
                if (distance < nearestDistance)
                {
                    nearestStation = station;
                    nearestDistance = distance;
                }
            }

            return nearestStation;
        }

        private double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        public void AddOrUpdateBike(Bike bike)
        {
            var existingBike = _bikeRepository.GetById(bike.Id);
            if (existingBike != null)
            {
                _bikeRepository.AddOrUpdate(bike);
            }
        }

        public void AddOrUpdateStation(Station station)
        {
            var existingStation = _stationRepository.GetStationById(station.Id);
            if (existingStation != null)
            {
                _stationRepository.AddOrUpdate(station);
            }
        }

        public Bike? GetBikeById(string id)
        {
            return string.IsNullOrEmpty(id) ? null : _bikeRepository.GetById(id);
        }

        public Station? GetStationById(string id)
        {
            return string.IsNullOrEmpty(id) ? null : _stationRepository.GetStationById(id);
        }

        public Bike? ChargeBike(string bikeId, string stationId)
        {
            Bike? bike = GetBikeById(bikeId);
            if (bike == null)
            {
                Console.WriteLine($"[Error] Bike with ID {bikeId} not found.");
                return null;
            }
            Station? station = GetStationById(stationId);
            if (station == null)
            {
                Console.WriteLine($"[Error] Station with ID {stationId} not found.");
                return null;
            }
            bike.X = station.Position.Item1;
            bike.Y = station.Position.Item2;
            _bikeRepository.AddOrUpdate(bike);
            _stationRepository.ChargeBike(bikeId, stationId);
            return bike;
        }
    }
}
