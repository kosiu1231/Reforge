﻿namespace Reforge.Services.GameService
{
    public interface IGameService
    {
        Task<ServiceResponse<GameDto>> AddGame(GameDto newGame);
        Task<ServiceResponse<List<GameDto>>> GetGames(QueryObject query);
        Task<ServiceResponse<List<GetModDto>>> GetGameMods(string name);
    }
}
