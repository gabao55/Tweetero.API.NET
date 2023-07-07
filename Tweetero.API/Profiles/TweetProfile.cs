using AutoMapper;

namespace Tweetero.API.Profiles
{
    public class TweetProfile : Profile
    {
        public TweetProfile() {
            CreateMap<Entities.Tweet, Models.TweetDto>();
        }
    }
}
