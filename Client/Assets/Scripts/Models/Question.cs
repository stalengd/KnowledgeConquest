using System.Collections.Generic;

namespace KnowledgeConquest.Client.Models
{
    public sealed class Question
    {
        public string Title { get; set; } = string.Empty;
        public List<QuestionAnswer> Answers { get; set; } = new();
    }

    public sealed class QuestionAnswer
    {
        public string Title { get; set; } = string.Empty;
    }
}
