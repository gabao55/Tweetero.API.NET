using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tweetero.API.Entities;
using Tweetero.API.Models;
using Tweetero.API.Repository;

namespace Tweetero.API.Controllers
{
    [Route("api/tweets")]
    [ApiController]
    public class TweetsController : Controller
    {
        private readonly ITweeteroRepository _repository;
        private readonly IMapper _mapper;

        public TweetsController(ITweeteroRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TweetDto>>> GetTweets()
        {
            IEnumerable<Tweet> tweets = await _repository.GetTweetsAsync();

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
