using Reforge.Dtos.Game;

namespace Reforge.Services.GameService
{
    public interface IGameService
    {
        Task<ServiceResponse<GameDto>> AddGame(GameDto newGame);
        Task<ServiceResponse<List<GetModDto>>> GetGameMods(string name);
    }
}
