using Microsoft.AspNetCore.Authorization;
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
    }
}
