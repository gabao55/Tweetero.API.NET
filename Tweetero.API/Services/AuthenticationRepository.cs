using Microsoft.EntityFrameworkCore;
using Tweetero.API.DbContexts;
using Tweetero.API.Entities;

namespace Tweetero.API.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly TweeteroContext _context;
        public AuthenticationRepository(TweeteroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<User> ValidateUser(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
