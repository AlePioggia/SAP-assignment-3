using StationService.domain.entities;

namespace StationService.application
{
    public interface IStationService
    {
        Task<Station> GetStationByIdAsync(string id);
        Task<IEnumerable<Station>> GetStationsAsync();
        Task<Station> GetNearestStation(int x, int y);
        Task<string> CreateStationAsync((int, int) position, int capacity);
        Task<string> DeleteStationAsync(string id);
        Task<string> chargeBike(string stationId, string bikeId);
    }
}
