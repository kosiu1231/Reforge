
using System.Security.Claims;

namespace Reforge.Services.ModService
{
    public class ModService : IModService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ModService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<ServiceResponse<GetModDto>> AddMod(AddModDto newMod)
        {
            var response = new ServiceResponse<GetModDto>();
            try
            {
                var mod = _mapper.Map<Mod>(newMod);
                mod.Creator = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
                mod.Game = await _context.Games.FirstOrDefaultAsync(g => g.Id == newMod.GameId);
               
                if(mod.Game is null)
                {
                    response.Success = false;
                    response.Message = "Game not found";
                    return response;
                }

                _context.Mods.Add(mod);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetModDto>(mod);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetModDto>>> GetMods()
        {
            var response = new ServiceResponse<List<GetModDto>>();
            try
            {
                var mods = await _context.Mods
                    .Include(g => g.Game)
                    .Include(c => c.Comments).ToListAsync();

                if (mods.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No mods found";
                    return response;
                }

                response.Data = mods.Select(m => _mapper.Map<GetModDto>(m)).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetModDto>> GetMod(int id)
        {
            var response = new ServiceResponse<GetModDto>();
            try
            {
                var mod = await _context.Mods
                    .Include(g => g.Game)
                    .Include(c => c.Comments)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (mod is null)
                {
                    response.Success = false;
                    response.Message = "Mod not found";
                    return response;
                }

                response.Data = _mapper.Map<GetModDto>(mod);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
