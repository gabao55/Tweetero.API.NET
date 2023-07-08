using Tweetero.API.Entities;

namespace Tweetero.API.Repository
{
    public interface ITweeteroRepository
    {
        Task<IEnumerable<Tweet>> GetTweetsAsync();
        Task<IEnumerable<Tweet>> GetUserTweetsAsync(int userId);
        Task<User> GetUserAsync(int userId);
    }
}
