namespace Reforge
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Mod, GetModDto>();
            CreateMap<AddModDto, Mod>();
            CreateMap<Game, GameDto>();
            CreateMap<Game, GameModsDto>();
            CreateMap<GameDto, Game>();
            CreateMap<AddCommentDto, Comment>();
            CreateMap<Comment, GetCommentDto>();
            CreateMap<User, GetUserDto>();
        }
    }
}
