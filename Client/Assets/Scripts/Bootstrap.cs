using UnityEngine;
using Zenject;
using KnowledgeConquest.Client.Connection;

namespace KnowledgeConquest.Client
{
    public sealed class Bootstrap : MonoBehaviour
    {
        private ApiConnection _apiConnection;

        [Inject]
        public void Construct(ApiConnection apiConnection)
        {
            _apiConnection = apiConnection;
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

            var isLoginSuccessful = await _apiConnection.LoginAsync();
            if (!isLoginSuccessful)
            {
                var isRegisterSuccessful = await _apiConnection.RegisterAsync();
                if (!isRegisterSuccessful)
                {
                    throw new System.Exception("Can not login nor register with provided credentials");
                }
            }

            Debug.Log("Logged in");
        }
    }
}
