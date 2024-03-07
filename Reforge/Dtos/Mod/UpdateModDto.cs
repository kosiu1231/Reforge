namespace Reforge.Dtos.Mod
{
    public class UpdateModDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Mod name has to be at least 3 characters")]
        [MaxLength(64, ErrorMessage = "Mod name has to be 64 characters at maximum")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MinLength(5, ErrorMessage = "Mod description has to be at least 3 characters")]
        [MaxLength(2000, ErrorMessage = "Mod description has to be 2000 characters at maximum")]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string? ImageUrl { get; set; } = "https://i.ibb.co/L0dNKsp/defaultimg.png";
    }
}
