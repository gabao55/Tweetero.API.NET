using Tweetero.API.Entities;
using Tweetero.API.Services;

namespace Tweetero.API.Repository
{
    public interface ITweeteroRepository
    {
        Task<(IEnumerable<Tweet>, PaginationMetadata)> GetTweetsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Tweet>> GetUserTweetsAsync(int userId);
        Task<User> GetUserAsync(int userId);
    }
}
