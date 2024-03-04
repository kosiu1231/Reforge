﻿using Microsoft.AspNetCore.Mvc;
using Reforge.Services.GameService;

namespace Reforge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GameDto>>> AddGame(GameDto newGame)
        {
            return Ok(await _gameService.AddGame(newGame));
        }
    }
}
