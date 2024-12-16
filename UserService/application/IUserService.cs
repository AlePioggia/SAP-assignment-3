using UserService.domain.entities;

namespace UserService.application
{
    public interface IUserService
    {
        Task<string> RegisterUser(string username, string password);
        Task<string> AuthenticateUser(string username, string password);
        Task<User> GetUserById(string id);
        Task<int> GetCredit(string id);
    }
}
