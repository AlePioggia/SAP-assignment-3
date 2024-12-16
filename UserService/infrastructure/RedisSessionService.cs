using StackExchange.Redis;
using UserService.application;

namespace UserService.Infrastructure
{
    public class RedisSessionService : ISessionService
    {
        private readonly IDatabase _database;

        public RedisSessionService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task StoreSessionAsync(string userId, string sessionId)
        {
            await _database.StringSetAsync(sessionId, userId, TimeSpan.FromHours(1));
        }

        public async Task<string> GetUserIdAsync(string sessionId)
        {
            return await _database.StringGetAsync(sessionId);
        }

        public async Task DeleteSessionAsync(string sessionId)
        {
            await _database.KeyDeleteAsync(sessionId);
        }
    }
}
