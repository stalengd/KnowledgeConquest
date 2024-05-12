using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KnowledgeConquest.Client.UI
{
    public class LoginRegisterPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _usernameInput;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private Toggle _registerToggle;
        [SerializeField] private Button _submitButton;

        private bool _autoClose = true;
        private TaskCompletionSource<Result> _taskCompletionSource;

        public struct Result
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public bool IsCreateAccount { get; set; }
        }

        private void Awake()
        {
            _submitButton.onClick.AddListener(SubmitButtonPressed);
        }

        public Task<Result> Open(bool autoClose = true) 
        {
            _autoClose = autoClose;
            _taskCompletionSource = new();
            PlayShowAnimation();
            return _taskCompletionSource.Task;
        }

        public void Close()
        {
            PlayHideAnimation();
            SetResult();
        }

        private void SetResult()
        {
            _taskCompletionSource.TrySetResult(new Result()
            {
                Username = _usernameInput.text,
                Password = _passwordInput.text,
                IsCreateAccount = _registerToggle.isOn,
            });
        }

        private void SubmitButtonPressed()
        {
            if (_autoClose)
            {
                Close();
            }
            else
            {
                SetResult();
            }
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
