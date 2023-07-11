using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Tweetero.API.Entities;
using Tweetero.API.Models;
using Tweetero.API.Repository;
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

        public abstract class RequestBody
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        public class AuthenticationRequestBody : RequestBody
        {
        }
        public class RegistrationRequestBody : RequestBody
        {
            public string? Avatar { get; set; }
        }

        private class TweeteroUser
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string? Avatar { get; set; }
        }

        public AuthenticationController(IConfiguration configuration,
            IAuthenticationRepository repository)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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

            string tokenToReturn = GenerateToken(user);

            return Ok(tokenToReturn);
        }

        private bool IsBodyValid(RequestBody body)
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

        private string GenerateToken(User user)
        {
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

            return tokenToReturn;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterUser([FromBody] RegistrationRequestBody body)
        {
            if (!IsBodyValid(body))
            {
                return BadRequest("Please provide valid username and password");
            }

            if (await _repository.UserExists(body.Username))
            {
                return Conflict("Username already taken, please try a different one!");
            }

            string hashedPassword = _passwordHasher.HashPassword(body.Username, body.Password);

            User newUser = new()
            {
                Username = body.Username,
                Password = hashedPassword,
                Avatar = body.Avatar
            };

            try
            {
                newUser = await _repository.CreateUser(newUser);

                string tokenToReturn = GenerateToken(newUser);

                return Created("api/authentication/register", tokenToReturn);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
