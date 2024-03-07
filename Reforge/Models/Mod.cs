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
        public List<Like>? Likes { get; set; } = new List<Like>();
        public int LikeAmount { get; set; } = 0;
        public string? ImageUrl { get; set; } = "https://i.ibb.co/L0dNKsp/defaultimg.png";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
