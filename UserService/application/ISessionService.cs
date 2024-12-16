namespace UserService.application
{
    public interface ISessionService
    {
        Task StoreSessionAsync(string userId, string sessionId);
        Task<string> GetUserIdAsync(string sessionId);
        Task DeleteSessionAsync(string sessionId);
    }
}
