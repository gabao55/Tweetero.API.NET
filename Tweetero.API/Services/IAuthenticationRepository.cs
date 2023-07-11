using Tweetero.API.Entities;

namespace Tweetero.API.Services
{
    public interface IAuthenticationRepository
    {
        public Task<User> ValidateUser(string username);
        public Task<bool> UserExists(string username);
        public Task<User> CreateUser(User newUser);
        Task<bool> SaveChangesAsync();
    }
}
