using Microsoft.AspNetCore.Identity;

namespace KnowledgeConquest.Server.Models
{
    public class User : IdentityUser
    {
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
    }
}
