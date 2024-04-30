using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KnowledgeConquest.Client.UI
{
    public class QuestionPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
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
            for (int i = 0; i < _process.Question.Answers.Count; i++)
            {
                var e = _answers.CreateElement();
                e.Render(i, _process.Question.Answers[i].Title, this);
                e.SetSelected(_process.SelectedAnswer == i);
            }
            _titleText.text = _process.Question.Title;
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

        private async void SubmitButtonPressed()
        {
            if (_process.SelectedAnswer < 0)
            {
                return;
            }
            await _process.EvaluateAsync();
            Close();
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
