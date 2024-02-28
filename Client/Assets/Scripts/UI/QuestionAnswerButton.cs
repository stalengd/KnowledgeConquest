using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace KnowledgeConquest.Client.UI
{
    public class QuestionAnswerButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _button;

        [Space]
        [SerializeField] private Graphic _tintTarget;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _unselectedColor;

        private int _questionIndex;
        private QuestionPanel _host;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }

        public void Render(int questionIndex, string title, QuestionPanel host)
        {
            _text.text = title;
            _questionIndex = questionIndex;
            _host = host;
        }

        public void SetSelected(bool isSelected)
        {
            _tintTarget.color = isSelected ? _selectedColor : _unselectedColor;
        }

        public void OnClick()
        {
            _host.AnswerSelected(_questionIndex);
        }
    }
}
