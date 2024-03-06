namespace Reforge.Dtos.Mod
{
    public class UpdateModDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = "https://i.ibb.co/L0dNKsp/defaultimg.png";
    }
}
