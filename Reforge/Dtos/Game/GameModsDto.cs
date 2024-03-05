using Reforge.Models;

namespace Reforge.Dtos.Game
{
    public class GameModsDto
    {
        public string Name { get; set; } = string.Empty;
        public List<GetModDto>? Mods { get; set; }
    }
}
