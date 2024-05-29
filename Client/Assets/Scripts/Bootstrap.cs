using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using KnowledgeConquest.Client.Connection;
using KnowledgeConquest.Client.UI;

namespace KnowledgeConquest.Client
{
    public sealed class Bootstrap : MonoBehaviour
    {
        [SerializeField] private string _loginErrorMessage = "Wrong username or password"; 

        private IApiConnection _apiConnection;
        private AccountApi _accountApi;
        private IMapLoader _mapLoader;
        private LoginRegisterPanel _loginRegisterPanel;
        private ErrorDisplay _errorDisplay;
        private LoadingIndicator _loadingIndicator;

        [Inject]
        public void Construct(
            IApiConnection apiConnection,
            AccountApi accountApi,
            IMapLoader mapLoader,
            LoginRegisterPanel loginRegisterPanel,
            ErrorDisplay errorDisplay,
            LoadingIndicator loadingIndicator)
        {
            _apiConnection = apiConnection;
            _accountApi = accountApi;
            _mapLoader = mapLoader;
            _loginRegisterPanel = loginRegisterPanel;
            _errorDisplay = errorDisplay;
            _loadingIndicator = loadingIndicator;
        }

        public void Start()
        {
            StartAsync();
        }

        private async void StartAsync()
        {
            _loadingIndicator.Show();
            if (!await _apiConnection.IsServerAvailiableAsync())
            {
                _errorDisplay.Display("Server is not availiable");
                _loadingIndicator.Hide();
                return;
            }
            _loadingIndicator.Hide();

            await HandleAuthenticationAsync();
            
            _loadingIndicator.Show();
            await _mapLoader.StartAsync();

            SceneManager.LoadScene("Main");
        }

        private async Task HandleAuthenticationAsync()
        {
            var isAuthenticated = false;
            while (!isAuthenticated)
            {
                var credentialsInput = await _loginRegisterPanel.Open(autoClose: false);
                _errorDisplay.Hide();
                _loadingIndicator.Show();

                if (credentialsInput.IsCreateAccount)
                {
                    var errors = await _accountApi.RegisterAsync(credentialsInput.Username, credentialsInput.Password);
                    if (errors == null || errors.Count == 0)
                    {
                        isAuthenticated = true;
                    }
                    else
                    {
                        isAuthenticated = false;
                        _errorDisplay.Display(errors[0].Description);
                    }
                }
                else
                {
                    isAuthenticated = await _accountApi.LoginAsync(credentialsInput.Username, credentialsInput.Password);
                    if (!isAuthenticated)
                    {
                        _errorDisplay.Display(_loginErrorMessage);
                    }
                }
                _loadingIndicator.Hide();
            }
            _loginRegisterPanel.Close();

            Debug.Log("Logged in");
        }
    }
}
