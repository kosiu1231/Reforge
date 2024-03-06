using Reforge.Dtos.User;

namespace Reforge.Dtos.Mod
{
    public class GetModDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<GetCommentDto>? Comments { get; set; }
        public GameDto? Game { get; set; }
        public GetUserDto? User { get; set; }
        public int LikeAmount { get; set; } = 0;
        public string? ImageUrl { get; set; } = string.Empty;
    }
}
