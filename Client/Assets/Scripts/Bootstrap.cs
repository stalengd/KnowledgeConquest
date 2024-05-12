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
        private IApiConnection _apiConnection;
        private AccountApi _accountApi;
        private MapApi _mapApi;
        private WorldMap _worldMap;
        private LoginRegisterPanel _loginRegisterPanel;
        private ErrorDisplay _errorDisplay;
        private LoadingIndicator _loadingIndicator;

        [Inject]
        public void Construct(
            IApiConnection apiConnection,
            AccountApi accountApi,
            MapApi mapApi,
            WorldMap worldMap,
            LoginRegisterPanel loginRegisterPanel,
            ErrorDisplay errorDisplay,
            LoadingIndicator loadingIndicator)
        {
            _apiConnection = apiConnection;
            _accountApi = accountApi;
            _mapApi = mapApi;
            _worldMap = worldMap;
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
            await LoadMapAsync();

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
                        _errorDisplay.Display("Wrong username or password");
                    }
                }
                _loadingIndicator.Hide();
            }
            _loginRegisterPanel.Close();

            Debug.Log("Logged in");
        }

        private async Task LoadMapAsync()
        {
            var map = await _mapApi.GetMapAsync();
            foreach (var cell in map.Cells)
            {
                if (cell.Type == Models.UserMapCellType.Owned)
                {
                    _worldMap.SetCellOwned(new(cell.PositionX, cell.PositionY));
                }
            }
        }
    }
}
