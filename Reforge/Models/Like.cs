namespace Reforge.Models
{
    public class Like
    {
        public int Id { get; set; }
        public User? User { get; set; }
        public Mod? Mod { get; set; }
    }
}
