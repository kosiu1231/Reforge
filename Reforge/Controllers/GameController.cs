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
            var response = await _gameService.AddGame(newGame);
            if(response.Data is null)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("game/{name:string}/mods")]
        public async Task<ActionResult<ServiceResponse<List<GetModDto>>>> GetGameMods(string name)
        {
            var response = await _gameService.GetGameMods(name);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
    }
}
