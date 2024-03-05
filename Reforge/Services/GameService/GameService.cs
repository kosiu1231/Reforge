
using Reforge.Models;

namespace Reforge.Services.GameService
{
    public class GameService : IGameService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GameService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<GameDto>> AddGame(GameDto newGame)
        {
            var response = new ServiceResponse<GameDto>();

            try
            {
                var game = new Game
                {
                    Name = newGame.Name
                };

                _context.Games.Add(game);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GameDto>(game);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetModDto>>> GetGameMods(string name)
        {
            var response = new ServiceResponse<List<GetModDto>>();

            try
            {
                var mods = await _context.Mods
                    .Include(g => g.Game)
                    .Include(c => c.Comments)
                    .Include(c => c.Creator)
                    .Where(m => m.Game!.Name == name).ToListAsync();

                if (mods.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Mods not found";
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
    }
}
