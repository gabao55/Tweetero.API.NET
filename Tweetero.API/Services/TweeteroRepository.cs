using Microsoft.EntityFrameworkCore;
using Tweetero.API.DbContexts;
using Tweetero.API.Entities;
using Tweetero.API.Repository;

namespace Tweetero.API.Services
{
    public class TweeteroRepository : ITweeteroRepository
    {
        private readonly TweeteroContext _context;
        public TweeteroRepository(TweeteroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<(IEnumerable<Tweet>, PaginationMetadata)> GetTweetsAsync(int pageNumber, int pageSize)
        {
            IEnumerable<Tweet> tweets = await _context.Tweets
                .Include(t => t.User)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            int totalItemCount = tweets.Count();

            PaginationMetadata paginationMetadata = new(totalItemCount, pageSize, pageNumber);

            return (tweets, paginationMetadata);
        }

        public async Task<User?> GetUserAsync(int userId) => await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        public async Task<IEnumerable<Tweet>> GetUserTweetsAsync(int userId)
        {
            return await _context.Tweets.Include(t => t.User).Where(t => t.UserId == userId).ToListAsync();
        }
    }
}
