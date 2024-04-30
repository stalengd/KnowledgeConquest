using UnityEngine;
using Zenject;
using KnowledgeConquest.Client.Connection;

namespace KnowledgeConquest.Client
{
    public sealed class Bootstrap : MonoBehaviour
    {
        private IApiConnection _apiConnection;
        private AccountApi _accountApi;
        private MapApi _mapApi;
        private WorldMap _worldMap;

        [Inject]
        public void Construct(IApiConnection apiConnection, AccountApi accountApi, MapApi mapApi, WorldMap worldMap)
        {
            _apiConnection = apiConnection;
            _accountApi = accountApi;
            _mapApi = mapApi;
            _worldMap = worldMap;
        }

        public void Start()
        {
            StartAsync();
        }

        private async void StartAsync()
        {
            if (!await _apiConnection.IsServerAvailiableAsync())
            {
                throw new System.Exception("Server is not availiable");
            }

            var isLoginSuccessful = await _accountApi.LoginAsync();
            if (!isLoginSuccessful)
            {
                var isRegisterSuccessful = await _accountApi.RegisterAsync();
                if (!isRegisterSuccessful)
                {
                    throw new System.Exception("Can not login nor register with provided credentials");
                }
            }

            Debug.Log("Logged in");

            var map = await _mapApi.GetMapAsync();
            foreach (var cell in map.Cells)
            {
                if (cell.Type == Models.UserMapCellType.Owned)
                {
                    _worldMap.SetCellOwned(new(cell.PositionX, cell.PositionY), redraw: false);
                }
            }
            _worldMap.Draw();
        }
    }
}
