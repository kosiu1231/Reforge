namespace Reforge.Models
{
    public class Mod
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public User? Creator { get; set; }
        public List<Comment>? Comments { get; set; }
        public Game? Game { get; set; }
        public int Likes { get; set; } = 0;
        public string? ImageUrl { get; set; } = string.Empty;
    }
}
