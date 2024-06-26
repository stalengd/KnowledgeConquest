﻿namespace KnowledgeConquest.Server.Models
{
    public sealed class UserInfoDTO
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
    }
}
