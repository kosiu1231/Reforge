namespace Reforge.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public User? User { get; set; }
        public Mod? Mod { get; set; }
        public int ModId { get; set; }
    }
}
