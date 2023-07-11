using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Tweetero.API.Entities;
using Tweetero.API.Models;
using Tweetero.API.Repository;
using Tweetero.API.Services;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace Tweetero.API.Controllers
{
    [Route("api/tweets")]
    [ApiController]
    public class TweetsController : Controller
    {
        private readonly ITweeteroRepository _repository;
        private readonly IMapper _mapper;
        const int maxTweetsPageSize = 50;

        public TweetsController(ITweeteroRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TweetDto>>> GetTweets([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            if (pageSize > maxTweetsPageSize) pageSize = maxTweetsPageSize;

            (IEnumerable<Tweet> tweets, PaginationMetadata paginationMetadata) = await _repository.GetTweetsAsync(pageNumber, pageSize);

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<TweetDto>>(tweets));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<TweetDto>>> GetUserTweets(int userId)
        {
            if (await _repository.GetUserAsync(userId) == null)
            {
                return NotFound();
            }

            IEnumerable<Tweet> tweets = await _repository.GetUserTweetsAsync(userId);

            return Ok(_mapper.Map<IEnumerable<TweetDto>>(tweets));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTweet([FromBody] TweetForCreationDto tweet)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<string> errorMessages = ModelState.Values.
                                                    SelectMany(v => v.Errors)
                                                    .Select(e => e.ErrorMessage);

                return BadRequest(errorMessages);
            }

            string? userId = User.Claims.FirstOrDefault(u => u.Type == "id")?.Value;

            if (userId == null)
                return BadRequest("Missing User Id in header");

            User user = await _repository.GetUserAsync(int.Parse(userId));

            if (user == null) return NotFound("User not found");

            try
            {
                Tweet createdTweet = await _repository.CreateTweet(tweet.Message, user);

                bool changesSaved = await _repository.SaveChangesAsync();

                if (!changesSaved && createdTweet == null) 
                    throw new Exception("The tweet could not be created, try again soon");

                TweetDto tweetToReturn = _mapper.Map<TweetDto>(createdTweet);

                return Created($"api/tweets/{userId}", tweetToReturn);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut("{tweetId}")]
        [Authorize]
        public async Task<IActionResult> UpdateTweet(int tweetId,
            [FromBody] TweetForUpdateDto tweetForUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            string? userId = User.Claims.FirstOrDefault(u => u.Type == "id")?.Value;

            if (userId == null)
                return BadRequest("Missing User Id in header");

            Tweet? tweet = await _repository.GetUserTweetAsync(int.Parse(userId), tweetId);

            if (tweet == null) return NotFound();

            try
            {
                _mapper.Map(tweetForUpdate, tweet);

                bool changesSaved = await _repository.SaveChangesAsync();

                if (!changesSaved)
                    throw new Exception("The tweet could not be created, try again soon");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{tweetId}")]
        [Authorize]
        public async Task<IActionResult> DeleteTweet([FromRoute] int tweetId)
        {
            string? userId = User.Claims.FirstOrDefault(u => u.Type == "id")?.Value;

            if (userId == null)
                return BadRequest("Missing User Id in header");

            Tweet? tweet = await _repository.GetUserTweetAsync(int.Parse(userId), tweetId);

            if (tweet == null) return NotFound();

            try
            {
                _repository.DeleteTweet(tweet);

                bool changesSaved = await _repository.SaveChangesAsync();

                if (!changesSaved)
                    throw new Exception("The tweet could not be created, try again soon");

                return Accepted();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
