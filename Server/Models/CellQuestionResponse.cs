using KnowledgeConquest.Shared.Math;

namespace KnowledgeConquest.Server.Models
{
    public record CellQuestionResponse(OffsetCoords CellPosition, int[] Answers)
    {
    }
}
