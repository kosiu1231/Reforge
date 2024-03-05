﻿namespace Reforge.Dtos.User
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public UserRole? Role { get; set; } = null;
    }
}
