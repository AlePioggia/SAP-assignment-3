using UserService.domain.entities;

namespace UserService.application
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> RegisterUser(string username, string password)
        {
            var user = new User(username, password);
            await _userRepository.Add(user);
            return user.Id;
        }

        public async Task<string> AuthenticateUser(string username, string password)
        {
            var user = await _userRepository.FindByUsername(username);
            if (user != null && user.VerifyPassword(password))
            {
                return user.Id;
            }
            return null;
        }

        public async Task<User> GetUserById(string id)
        {
            return await _userRepository.FindById(id);
        }

        public async Task<int> GetCredit(string userId)
        {
            return await _userRepository.GetCredit(userId);
        }
    }
}
