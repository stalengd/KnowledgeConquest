﻿using System.ComponentModel.DataAnnotations.Schema;

namespace KnowledgeConquest.Server.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        [InverseProperty(nameof(QuestionAnswer.Question))]
        public List<QuestionAnswer> Answers { get; set; } = new();

        public int CheckAnswers(IReadOnlyList<int> answerNumbers)
        {
            var right = 0;
            for (int i = 0; i < Answers.Count; i++)
            {
                if (answerNumbers.Contains(Answers[i].Index))
                    right++;
            }
            return right;
        }
    }
}
