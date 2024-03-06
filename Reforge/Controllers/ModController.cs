﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reforge.Services.ModService;

namespace Reforge.Controllers
{
    [ApiController]
    [Route("api")]
    public class ModController : ControllerBase
    {
        private readonly IModService _modService;

        public ModController(IModService modService)
        {
            _modService = modService;
        }

        [Authorize]
        [HttpPost("mod")]
        public async Task<ActionResult<ServiceResponse<GetModDto>>> AddMod(AddModDto newMod)
        {
            var response = await _modService.AddMod(newMod);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("mods")]
        public async Task<ActionResult<ServiceResponse<List<GetModDto>>>> GetMods()
        {
            var response = await _modService.GetMods();
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("mod/{id}")]
        public async Task<ActionResult<ServiceResponse<GetModDto>>> GetMod(int id)
        {
            var response = await _modService.GetMod(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("mod/{id}/like")]
        public async Task<ActionResult<ServiceResponse<GetModDto>>> LikeMod(int id)
        {
            var response = await _modService.LikeMod(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("mod/{id}/dislike")]
        public async Task<ActionResult<ServiceResponse<GetModDto>>> DislikeMod(int id)
        {
            var response = await _modService.DislikeMod(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
    }
}
