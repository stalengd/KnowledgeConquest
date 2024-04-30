using UnityEngine;
using Zenject;
using KnowledgeConquest.Client.Connection;

namespace KnowledgeConquest.Client.DI
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private ConnectionConfig _connectionConfig;

        public override void InstallBindings()
        {
            BindApiConnection();
            BindConnectionConfig();
            BindMapApi();
            BindAccountApi();
        }

        private void BindApiConnection()
        {
            Container
                .Bind<IApiConnection>()
                .To<ApiConnection>()
                .AsSingle();
        }

        private void BindConnectionConfig()
        {
            Container
                .Bind<IConnectionConfig>()
                .To<ConnectionConfig>()
                .FromInstance(_connectionConfig);
        }

        private void BindAccountApi()
        {
            Container
                .Bind<AccountApi>()
                .AsSingle();
        }

        private void BindMapApi()
        {
            Container
                .Bind<MapApi>()
                .AsSingle();
        }
    }
}