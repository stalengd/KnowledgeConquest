using UnityEngine;

namespace KnowledgeConquest.Client
{
    public class QuestionsRepository 
    {
        private readonly Question _dummyQuestion = new()
        {
            Text = "Test Question",
            Answers = new[]
            {
                "Wrong",
                "Right",
                "Wrong",
                "Wrong",
            },
            CorrectAnswer = 1
        };

        public Question GetQuestion(Vector2Int cell)
        {
            return _dummyQuestion;
        }
    }
}
