
namespace UserService.domain.entities
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public int Credit { get; set; }
        public DateTime CreatedAt { get; set; }


        public User(string username, string password)
        {
            Id = Guid.NewGuid().ToString();
            Username = username;
            PasswordHash = HashPassword(password);
            Credit = 0;
            CreatedAt = DateTime.Now;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }
    }

}
