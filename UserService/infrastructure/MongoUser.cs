using MongoDB.Bson.Serialization.Attributes;
using UserService.domain.entities;

namespace UserService.infrastructure
{
    public class MongoUser
    {
        [BsonId]
        public string Id { get; set; }

        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public int Credit {  get; set; }
        public DateTime CreatedAt { get; set; }

        public User ToDomainUser()
        {
            var user = new User(Username, null)
            {
                Id = Id,
                PasswordHash = PasswordHash,
                CreatedAt = CreatedAt
            };
            return user;
        }

        public static MongoUser FromDomainUser(User user)
        {
            return new MongoUser
            {
                Id = user.Id,
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                Credit = user.Credit,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
