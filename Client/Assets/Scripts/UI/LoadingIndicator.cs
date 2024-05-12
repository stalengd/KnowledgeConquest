using UnityEngine;

namespace KnowledgeConquest.Client.UI
{
    public sealed class LoadingIndicator : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
