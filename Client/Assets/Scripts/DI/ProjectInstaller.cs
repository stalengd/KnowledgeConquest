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
            BindAPIConnection();
            BindConnectionConfig();
        }

        private void BindAPIConnection()
        {
            Container
                .Bind<ApiConnection>()
                .AsSingle();
        }

        private void BindConnectionConfig()
        {
            Container
                .Bind<IConnectionConfig>()
                .To<ConnectionConfig>()
                .FromInstance(_connectionConfig);
        }
    }
}