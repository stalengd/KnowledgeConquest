namespace KnowledgeConquest.Server.Models
{
    public sealed class QuestionDTO
    {
        public string Title { get; set; } = string.Empty;
        public List<QuestionAnswerDTO> Answers { get; set; } = new();
    }

    public sealed class QuestionAnswerDTO
    {
        public string Title { get; set; } = string.Empty;
    }
}
