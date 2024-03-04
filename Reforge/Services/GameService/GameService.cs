
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
    }
}
