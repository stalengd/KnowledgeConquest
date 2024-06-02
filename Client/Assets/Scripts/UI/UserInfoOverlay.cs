using TMPro;
using UnityEngine;

namespace KnowledgeConquest.Client.UI
{
    public sealed class UserInfoOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _positiveScoreText;
        [SerializeField] private TMP_Text _negativeScoreText;

        public void Render(User user)
        {
            _nameText.text = $"{user.Firstname} {user.Surname}";
            _positiveScoreText.text = user.Map.CountState(UserMap.CellState.CapturedSuccessfuly).ToString();
            _negativeScoreText.text = user.Map.CountState(UserMap.CellState.CapturedFaily).ToString();
        }
    }
}
