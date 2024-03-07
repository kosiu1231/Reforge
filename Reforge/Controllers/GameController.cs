using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reforge.Services.GameService;

namespace Reforge.Controllers
{
    [ApiController]
    [Route("api")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [Authorize]
        [HttpPost("game")]
        public async Task<ActionResult<ServiceResponse<GameDto>>> AddGame(GameDto newGame)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _gameService.AddGame(newGame);
            if(response.Data is null)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("games")]
        public async Task<ActionResult<ServiceResponse<List<GameDto>>>> GetGames([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _gameService.GetGames(query);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("game/{name}/mods")]
        public async Task<ActionResult<ServiceResponse<List<GetModDto>>>> GetGameMods(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _gameService.GetGameMods(name);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
    }
}
