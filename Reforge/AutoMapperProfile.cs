namespace Reforge
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Mod, GetModDto>();
            CreateMap<AddModDto, Mod>();
            CreateMap<Game, GameDto>();
            CreateMap<GameDto, Game>();
        }
    }
}
