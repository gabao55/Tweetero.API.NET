using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tweetero.API.Entities;
using Tweetero.API.Services;

namespace Tweetero.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationRepository _repository;
        private readonly PasswordHasher<string> _passwordHasher = new();
        public class AuthenticationRequestBody
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
        }
        public class RegistrationRequestBody
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string? Avatar { get; set; }
        }

        private class TweeteroUser
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string? Avatar { get; set; }
        }

        public AuthenticationController(IConfiguration configuration, IAuthenticationRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate([FromBody] AuthenticationRequestBody body)
        {
            if (!IsBodyValid(body))
            {
                return Unauthorized("Username or password invalid");
            }

            User user = await ValidateCredentials(body.Username, body.Password);

            if (user == null)
            {
                return Unauthorized("Username or password invalid");
            }

            SymmetricSecurityKey securityKey = new(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            SigningCredentials signingCredentials = new(
                securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claimsForToken = new();
            claimsForToken.Add(new Claim("id", user.Id.ToString()));
            claimsForToken.Add(new Claim("username", user.Username));
            claimsForToken.Add(new Claim("avatar", user.Avatar));

            JwtSecurityToken jwtToken = new(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(2),
                signingCredentials);

            string tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtToken);

            return Ok(tokenToReturn);
        }

        private bool IsBodyValid(AuthenticationRequestBody body)
        {
            if (body == null ||
                String.IsNullOrEmpty(body.Username) ||
                String.IsNullOrEmpty(body.Password)) return false;

            return true;
        }

        private async Task<User?> ValidateCredentials(string username, string password)
        {
            User user = await _repository.ValidateUser(username);

            if (user == null) return null;

            PasswordVerificationResult passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user.Username, user.Password, password);

            if (passwordVerificationResult != PasswordVerificationResult.Success) return null;

            return user;
        }
    }
}
