namespace Reforge.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Mod>? Mods { get; set; }
    }
}
