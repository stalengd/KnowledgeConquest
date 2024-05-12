using TMPro;
using UnityEngine;

namespace KnowledgeConquest.Client.UI
{
    public sealed class ErrorDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void Display(string message)
        {
            gameObject.SetActive(true);
            _text.text = message;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
