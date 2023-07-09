using Tweetero.API.Entities;

namespace Tweetero.API.Services
{
    public interface IAuthenticationRepository
    {
        public Task<User> ValidateUser(string username);
    }
}
