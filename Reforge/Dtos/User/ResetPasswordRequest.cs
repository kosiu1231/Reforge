﻿namespace Reforge.Dtos.User
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required]
        [MinLength(6, ErrorMessage = "Password has to be at least 6 characters")]
        [MaxLength(32, ErrorMessage = "Password has to be 32 characters at maximum")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
