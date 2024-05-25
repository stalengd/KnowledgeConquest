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

            BindWorldMap();
            BindMapLoader();
            BindUsersRepository();
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

        private void BindWorldMap()
        {
            Container
                .Bind<WorldMap>()
                .AsSingle();
        }

        private void BindMapLoader()
        {
            Container
                .BindInterfacesTo<MapLoader>()
                .AsSingle();
        }

        private void BindUsersRepository()
        {
            Container
                .BindInterfacesTo<UsersRepository>()
                .AsSingle();
        }
    }
}