using TMPro;
using UnityEngine;

namespace KnowledgeConquest.Client.UI
{
    public sealed class UserInfoOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;

        public void Render(User user)
        {
            _nameText.text = user.Username;
        }
    }
}
