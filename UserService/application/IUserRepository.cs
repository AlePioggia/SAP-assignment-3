using UserService.domain.entities;

namespace UserService.application
{
    public interface IUserRepository
    {
        Task Add(User user);
        Task<User> FindByUsername(string username);
        Task<User> FindById(string id);
        Task<int> GetCredit(string id);
    }
}
