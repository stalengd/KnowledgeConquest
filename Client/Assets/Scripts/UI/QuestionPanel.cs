using UnityEngine;
using UnityEngine.UI;

namespace KnowledgeConquest.Client.UI
{
    public class QuestionPanel : MonoBehaviour
    {
        [SerializeField] private ListHelper<QuestionAnswerButton> _answers;
        [SerializeField] private Button _submitButton;
        private QuestionProcess _process;

        private void Awake()
        {
            _submitButton.onClick.AddListener(SubmitButtonPressed);
        }

        public void Open(QuestionProcess questionProcess)
        {
            _process = questionProcess;
            _answers.Clear();
            for (int i = 0; i < _process.Question.Answers.Length; i++)
            {
                var e = _answers.CreateElement();
                e.Render(i, _process.Question.Answers[i], this);
                e.SetSelected(_process.SelectedAnswer == i);
            }
            PlayShowAnimation();
        }

        public void Close()
        {
            PlayHideAnimation();
        }

        public void AnswerSelected(int index)
        {
            if (_process.SelectedAnswer >= 0)
            {
                _answers[_process.SelectedAnswer].SetSelected(false);
            }
            _process.SelectedAnswer = index;
            _answers[index].SetSelected(true);
        }

        private void SubmitButtonPressed()
        {
            Close();
            _process.Evaluate();
        }

        private void PlayShowAnimation()
        {
            gameObject.SetActive(true);
        }

        private void PlayHideAnimation()
        {
            gameObject.SetActive(false);
        }
    }
}
