using Tweetero.API.Entities;

namespace Tweetero.API.Models
{
    public class TweetDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
    }
}
