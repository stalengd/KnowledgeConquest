namespace KnowledgeConquest.UtilityCli.Models;

public sealed class Question
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<QuestionAnswer> Answers { get; set; } = new();
}

public sealed class QuestionAnswer
{
    public int QuestionId { get; set; }
    public int Index { get; set; }
    public bool IsRight { get; set; }
    public string Title { get; set; } = "";
}
