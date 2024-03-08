namespace Reforge.Services.CommentService
{
    public interface ICommentService
    {
        Task<ServiceResponse<GetModDto>> AddComment(AddCommentDto newComment);
        Task<ServiceResponse<List<GetCommentDto>>> GetUserComments(string name);
        Task<ServiceResponse<GetModDto>> DeleteComment(int id);
        Task<ServiceResponse<GetModDto>> UpdateComment(UpdateCommentDto updatedComment);
    }
}
