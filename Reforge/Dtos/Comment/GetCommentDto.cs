using Reforge.Dtos.User;

namespace Reforge.Dtos.Comment
{
    public class GetCommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int ModId { get; set; }
        public GetUserDto? User { get; set; }
    }
}
