using System.ComponentModel.DataAnnotations;

namespace Tweetero.API.Models
{
    public class TweetForCreationDto
    {
        [Required(ErrorMessage="You should provide a message")]
        public string Message { get; set; }
    }
}
