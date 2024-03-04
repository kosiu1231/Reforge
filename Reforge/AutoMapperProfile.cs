using Reforge.Dtos.Mod;

namespace Reforge
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Mod, GetModDto>();
        }
    }
}
