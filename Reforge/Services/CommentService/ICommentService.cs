using Microsoft.AspNetCore.Mvc;

namespace Reforge.Services.CommentService
{
    public interface ICommentService
    {
        Task<ServiceResponse<GetModDto>> AddComment(AddCommentDto newComment);
        Task<ServiceResponse<List<GetCommentDto>>> GetUserComments(string name);
    }
}
