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

        public async Task<User> CreateUser(User newUser)
        {
            await _context.Users.AddAsync(newUser);

            bool changesSaved = await SaveChangesAsync();

            if (!changesSaved && newUser == null)
                throw new Exception("The user could not be registered, try again soon");

            return await _context.Users.OrderBy(u => u.Id).LastAsync();
        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username);
        }

        public async Task<User> ValidateUser(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
