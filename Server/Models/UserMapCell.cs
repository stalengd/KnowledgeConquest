namespace KnowledgeConquest.Server.Models
{
    public class UserMapCell
    {
        public User User { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public int PositionX { get; set; } 
        public int PositionY { get; set; } 
        public UserMapCellType Type { get; set; }
        public Question Question { get; set; } = null!;
        public int QuestionId { get; set; }
    }

    public enum UserMapCellType
    {
        Free = 0,
        Owned = 1
    }
}
