using MongoDB.Driver;
using UserService.application;
using UserService.domain.entities;

namespace UserService.infrastructure
{
    public class UserRepository: IUserRepository
    {
        private readonly IMongoCollection<MongoUser> _users;

        public UserRepository(IMongoClient mongoClient) 
        {
            var database = mongoClient.GetDatabase("UserServiceDb");
            _users = database.GetCollection<MongoUser>("Users");
        }

        public async Task Add(User user)
        {
            var mongoUser = MongoUser.FromDomainUser(user);
            await _users.InsertOneAsync(mongoUser);
        }

        public async Task<User> FindById(string id)
        {
            var filter = Builders<MongoUser>.Filter.Eq(u => u.Id, id);
            var mongoUser = await _users.Find(filter).FirstOrDefaultAsync();
            return mongoUser.ToDomainUser();
        }

        public async Task<User> FindByUsername(string username)
        {
            var filter = Builders<MongoUser>.Filter.Eq(u => u.Username, username);
            var mongoUser = await _users.Find(filter).FirstOrDefaultAsync();
            return mongoUser.ToDomainUser();
        }

        public async Task<int> GetCredit(string userId)
        {
            var filter = Builders<MongoUser>.Filter.Eq(u => u.Id, userId);
            var projection = Builders<MongoUser>.Projection.Expression(u => u.Credit);
            var credit = await _users.Find(filter).Project(projection).FirstOrDefaultAsync();
            return credit;
        }
    }
}
