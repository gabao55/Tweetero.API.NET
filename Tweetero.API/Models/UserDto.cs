namespace Tweetero.API.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public ICollection<TweetDto> Tweets { get; set; }
    }
}
