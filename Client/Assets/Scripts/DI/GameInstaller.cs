using UnityEngine;
using Zenject;
using KnowledgeConquest.Client.UI;

namespace KnowledgeConquest.Client.DI
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private WorldMapRenderer _worldMapRenderer;
        [SerializeField] private QuestionPanel _questionPanel;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private LoadingIndicator _loadingIndicator;

        public override void InstallBindings()
        {
            Container
                .Bind<WorldMapRenderer>()
                .FromInstance(_worldMapRenderer);
            Container
                .Bind<QuestionPanel>()
                .FromInstance(_questionPanel);
            Container
                .Bind<QuestionsRepository>()
                .AsSingle();
            Container
                .Bind<CameraController>()
                .FromInstance(_cameraController);
            Container
                .Bind<LoadingIndicator>()
                .FromInstance(_loadingIndicator);
        }
    }
}