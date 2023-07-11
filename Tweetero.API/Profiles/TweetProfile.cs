using AutoMapper;

namespace Tweetero.API.Profiles
{
    public class TweetProfile : Profile
    {
        public TweetProfile() {
            CreateMap<Entities.Tweet, Models.TweetDto>()
                .ForMember(tweetDto => tweetDto.Username, opt => opt.MapFrom(tweet => tweet.User.Username))
                .ForMember(tweetDto => tweetDto.Avatar, opt => opt.MapFrom(tweet => tweet.User.Avatar));
            CreateMap<Models.TweetForUpdateDto, Entities.Tweet>();
        }
    }
}
