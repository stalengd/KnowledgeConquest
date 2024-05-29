using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KnowledgeConquest.Client.UI
{
    public class LoginRegisterPanel : MonoBehaviour
    {
        [SerializeField] private DualToggle _registerModeToggle;

        [Header("Login Page")]
        [SerializeField] private GameObject _loginPage;
        [SerializeField] private TMP_InputField _loginUsernameInput;
        [SerializeField] private TMP_InputField _loginPasswordInput;
        [SerializeField] private Button _loginSubmitButton;

        [Header("Register Page")]
        [SerializeField] private GameObject _registerPage;
        [SerializeField] private TMP_InputField _registerUsernameInput;
        [SerializeField] private TMP_InputField _registerFirstnameInput;
        [SerializeField] private TMP_InputField _registerSurnameInput;
        [SerializeField] private TMP_InputField _registerPasswordInput;
        [SerializeField] private TMP_InputField _registerPassword2Input;
        [SerializeField] private Button _registerSubmitButton;

        private bool _autoClose = true;
        private TaskCompletionSource<Result> _taskCompletionSource;

        public struct Result
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Firstname { get; set; }
            public string Surname { get; set; }
            public bool IsCreateAccount { get; set; }
        }

        private void Awake()
        {
            _loginSubmitButton.onClick.AddListener(SubmitButtonPressed);
            _registerSubmitButton.onClick.AddListener(SubmitButtonPressed);
            _registerModeToggle.ValueChanged += isRegister =>
            {
                _loginPage.SetActive(!isRegister);
                _registerPage.SetActive(isRegister);
            };
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
            var r = new Result()
            {
                IsCreateAccount = _registerModeToggle.Value,
            };
            if (r.IsCreateAccount)
            {
                if (_registerPasswordInput.text != _registerPassword2Input.text)
                {
                    return;
                }
                r.Username = _registerUsernameInput.text;
                r.Firstname = _registerFirstnameInput.text;
                r.Surname = _registerSurnameInput.text;
                r.Password = _registerPasswordInput.text;
            }
            else
            {
                r.Username = _loginUsernameInput.text;
                r.Password = _loginPasswordInput.text;
            }
            _taskCompletionSource.TrySetResult(r);
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
