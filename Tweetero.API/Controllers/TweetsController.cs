using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Tweetero.API.Entities;
using Tweetero.API.Models;
using Tweetero.API.Repository;
using Tweetero.API.Services;

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
            _repository = repository;
            _mapper = mapper;
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
    }
}
