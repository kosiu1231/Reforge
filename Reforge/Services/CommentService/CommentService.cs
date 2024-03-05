

using System.Security.Claims;

namespace Reforge.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<ServiceResponse<GetModDto>> AddComment(AddCommentDto newComment)
        {
            var response = new ServiceResponse<GetModDto>();
            try
            {
                var comment = _mapper.Map<Comment>(newComment);
                var mod = await _context.Mods.FirstOrDefaultAsync(m => m.Id == comment.ModId);
                comment.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
                comment.Mod = mod;

                if (mod is null)
                {
                    response.Success = false;
                    response.Message = "Mod not found";
                    return response;
                }

                _context.Comments.Add(comment);
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

        public async Task<ServiceResponse<List<GetCommentDto>>> GetUserComments(string name)
        {
            var response = new ServiceResponse<List<GetCommentDto>>();

            try
            {
                var comments = await _context.Comments
                    .Where(c => c.User!.Username == name).ToListAsync();

                if (comments.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Comments not found";
                    return response;
                }

                response.Data = comments.Select(c => _mapper.Map<GetCommentDto>(c)).ToList();
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
