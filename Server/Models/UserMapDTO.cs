namespace KnowledgeConquest.Server.Models
{
    public sealed class UserMapDTO
    {
        public string UserId { get; set; } = null!;
        public List<UserMapCellDTO> Cells { get; set; } = new();
    }

    public sealed class UserMapCellDTO
    {
        public int PositionX { get; set; } 
        public int PositionY { get; set; } 
        public UserMapCellType Type { get; set; }
    }
}
