namespace Reforge.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CommentService> _logger;

        public CommentService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, ILogger<CommentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);

        private string GetUserRole() => _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.Role)!;

        public async Task<ServiceResponse<GetModDto>> AddComment(AddCommentDto newComment)
        {
            var response = new ServiceResponse<GetModDto>();
            try
            {
                var comment = _mapper.Map<Comment>(newComment);
                var mod = await _context.Mods
                    .Include(c => c.Creator)
                    .Include(g => g.Game)
                    .Include(c => c.Comments)
                    .FirstOrDefaultAsync(m => m.Id == comment.ModId);
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
            _logger.LogInformation($"AddComment(Mod={newComment.ModId}, {newComment.Text}) invoked by {GetUserId()}.");
            return response;
        }

        public async Task<ServiceResponse<List<GetCommentDto>>> GetUserComments(string name)
        {
            var response = new ServiceResponse<List<GetCommentDto>>();

            try
            {
                var comments = await _context.Comments
                    .Include(u => u.User)
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
            _logger.LogInformation($"GetUserComments({name}) invoked.");
            return response;
        }

        public async Task<ServiceResponse<GetModDto>> DeleteComment(int id)
        {
            var response = new ServiceResponse<GetModDto>();
            string logMessage = "";
            try
            {
                //User who created comment or admin
                var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id
                && (c.User!.Id == GetUserId() || GetUserRole() == "Admin"));

                if (comment is null)
                {
                    response.Success = false;
                    response.Message = "Comment not found";
                    return response;
                }

                logMessage = comment.Text;

                var mod = await _context.Mods
                    .Include(c => c.Creator)
                    .Include(g => g.Game)
                    .Include(c => c.Comments)
                    .FirstOrDefaultAsync(m => m.Comments!.Any(c => c.Id == id));

                if (mod is null)
                {
                    response.Success = false;
                    response.Message = "Mod not found";
                    return response;
                }

                mod!.Comments!.Remove(comment);
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetModDto>(mod);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            _logger.LogInformation($"DeleteComment({logMessage}) invoked by {GetUserId()}.");
            return response;
        }

        public async Task<ServiceResponse<GetModDto>> UpdateComment(UpdateCommentDto updatedComment)
        {
            var response = new ServiceResponse<GetModDto>();
            try
            {
                //User who created comment or admin
                var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == updatedComment.Id
                && (c.User!.Id == GetUserId() || GetUserRole() == "Admin"));

                if (comment is null)
                {
                    response.Success = false;
                    response.Message = "Comment not found";
                    return response;
                }

                var mod = await _context.Mods
                    .Include(c => c.Creator)
                    .Include(g => g.Game)
                    .Include(c => c.Comments)
                    .FirstOrDefaultAsync(m => m.Comments!.Any(c => c.Id == updatedComment.Id));

                if (mod is null)
                {
                    response.Success = false;
                    response.Message = "Mod not found";
                    return response;
                }

                comment.Text = updatedComment.Text;
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetModDto>(mod);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            _logger.LogInformation($"UpdateComment(ID={updatedComment.Id}, {updatedComment.Text}) invoked by {GetUserId()}.");
            return response;
        }
    }
}
