namespace KnowledgeConquest.Client
{
    public class QuestionProcess 
    {
        public Question Question { get; }
        public int SelectedAnswer { get; set; } = -1;
        public bool? Result { get; private set; } = null;

        public event System.Action<bool> OnAnswered;

        public QuestionProcess(Question question)
        {
            Question = question;
        }

        public bool Evaluate()
        {
            if (Result != null) throw new System.Exception("Already evaluated.");
            if (SelectedAnswer < 0 || SelectedAnswer >= Question.Answers.Length) throw new System.Exception("Answer is not selected properly.");

            var result = Question.CorrectAnswer == SelectedAnswer;
            Result = result;
            OnAnswered?.Invoke(result);

            return result;
        }
    }
}
