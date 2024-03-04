﻿namespace Reforge
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Mod, GetModDto>();
            CreateMap<Game, GetGameDto>();
        }
    }
}
