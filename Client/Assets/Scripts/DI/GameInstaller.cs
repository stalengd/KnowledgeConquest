using UnityEngine;
using Zenject;
using KnowledgeConquest.Client.UI;

namespace KnowledgeConquest.Client.DI
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private WorldMap _worldMap;
        [SerializeField] private QuestionPanel _questionPanel;

        public override void InstallBindings()
        {
            Container
                .Bind<WorldMap>()
                .FromInstance(_worldMap);
            Container
                .Bind<QuestionPanel>()
                .FromInstance(_questionPanel);
            Container
                .Bind<QuestionsRepository>()
                .AsSingle();
        }
    }
}