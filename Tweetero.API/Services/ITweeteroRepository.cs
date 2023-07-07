using Tweetero.API.Entities;

namespace Tweetero.API.Repository
{
    public interface ITweeteroRepository
    {
        Task<IEnumerable<Tweet>> GetTweetsAsync();
    }
}
