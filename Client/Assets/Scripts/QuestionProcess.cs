using System.Threading.Tasks;
using UnityEngine;
using KnowledgeConquest.Client.Connection;
using KnowledgeConquest.Client.Models;

namespace KnowledgeConquest.Client
{
    public class QuestionProcess 
    {
        public Vector2Int CellPosition { get; }
        public Question Question { get; }
        public int SelectedAnswer { get; set; } = -1;
        public bool? Result { get; private set; } = null;

        public event System.Action<bool> OnAnswered;

        private readonly MapApi _mapApi;

        public QuestionProcess(Vector2Int cellPosition, Question question, MapApi mapApi)
        {
            CellPosition = cellPosition;
            Question = question;
            _mapApi = mapApi;
        }

        public async Task<bool> EvaluateAsync()
        {
            if (Result != null)
            {
                throw new System.Exception("Already evaluated.");
            }
            if (SelectedAnswer < 0 || SelectedAnswer >= Question.Answers.Count)
            {
                throw new System.Exception("Answer is not selected properly.");
            }

            var result = await _mapApi.AnswerQuestionAsync(CellPosition, new[] { SelectedAnswer });
            Result = result;
            OnAnswered?.Invoke(result);

            return result;
        }
    }
}
