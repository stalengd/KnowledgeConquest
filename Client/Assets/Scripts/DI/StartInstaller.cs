using UnityEngine;
using Zenject;
using KnowledgeConquest.Client.UI;

namespace KnowledgeConquest.Client.DI
{
    public class StartInstaller : MonoInstaller
    {
        [SerializeField] private LoginRegisterPanel _loginRegisterPanel;
        [SerializeField] private ErrorDisplay _errorDisplay;
        [SerializeField] private LoadingIndicator _loadingIndicator;

        public override void InstallBindings()
        {
            Container
                .Bind<LoginRegisterPanel>()
                .FromInstance(_loginRegisterPanel);
            Container
                .Bind<ErrorDisplay>()
                .FromInstance(_errorDisplay);
            Container
                .Bind<LoadingIndicator>()
                .FromInstance(_loadingIndicator);
        }
    }
}