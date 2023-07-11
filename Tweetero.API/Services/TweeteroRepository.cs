using Microsoft.EntityFrameworkCore;
using Tweetero.API.DbContexts;
using Tweetero.API.Entities;
using Tweetero.API.Models;
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

        public async Task<Tweet> CreateTweet(string message, User user)
        {
            user.Tweets.Add(new Tweet(message));

            return user.Tweets.OrderBy(t => t.Id).Last();
        }

        public async Task<Tweet?> GetUserTweetAsync(int userId, int tweetId)
        {
            return await _context.Tweets
                                 .Include(t => t.User)
                                 .FirstOrDefaultAsync(t => t.Id == tweetId && t.UserId == userId);
        }

        public void DeleteTweet(Tweet tweet)
        {
            _context.Tweets.Remove(tweet);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
