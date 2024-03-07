namespace KnowledgeConquest.Server.Models
{
    public class QuestionAnswer
    {
        public int QuestionId { get; set; }
        public int Index { get; set; }
        public Question Question { get; set; } = null!;
        public bool IsRight { get; set; }
        public string Title { get; set; } = "";
    }
}
