namespace Reforge.Controllers
{
    [ApiController]
    [Route("api")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("user/{name}/comments")]
        public async Task<ActionResult<ServiceResponse<List<GetCommentDto>>>> GetUserComments(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _commentService.GetUserComments(name);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("comment")]
        public async Task<ActionResult<ServiceResponse<GetModDto>>> AddComment(AddCommentDto newComment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _commentService.AddComment(newComment);
            if (response.Data is null)
                return BadRequest(response);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("comment")]
        public async Task<ActionResult<ServiceResponse<GetModDto>>> DeleteComment(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _commentService.DeleteComment(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [Authorize]
        [HttpPut("comment")]
        public async Task<ActionResult<ServiceResponse<GetModDto>>> UpdateComment(UpdateCommentDto updatedComment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _commentService.UpdateComment(updatedComment);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
    }
}