using System.ComponentModel.DataAnnotations;

namespace KnowledgeConquest.Server.Models
{
    public record AccountRegisterModel(
        string Username, 
        string Password, 
        [StringLength(maximumLength: 32, MinimumLength = 2)]
        string Firstname, 
        [StringLength(maximumLength: 32, MinimumLength = 2)]
        string Surname);
}
