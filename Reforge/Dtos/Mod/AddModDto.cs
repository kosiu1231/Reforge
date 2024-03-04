namespace Reforge.Dtos.Mod
{
    public class AddModDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int GameId { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
    }
}
