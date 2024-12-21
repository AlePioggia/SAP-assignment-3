using StationService.domain.entities;

namespace StationService.application
{
    public interface IStationRepository
    {
        Task<Station> FindById(string id);
        Task<IEnumerable<Station>> FindAll();
        Task<Station> Add(Station station);
        Task<Station> Update(Station station);
        Task<Station> Delete(string id);
    }
}
