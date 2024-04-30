using System.Collections.Generic;

namespace KnowledgeConquest.Client.Models
{
    public sealed class UserMap
    {
        public string UserId { get; set; } = null!;
        public List<UserMapCell> Cells { get; set; } = new();
    }

    public sealed class UserMapCell
    {
        public int PositionX { get; set; } 
        public int PositionY { get; set; } 
        public UserMapCellType Type { get; set; }
    }
}
