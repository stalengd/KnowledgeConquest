using UnityEngine;
using Zenject;
using KnowledgeConquest.Client.UI;
using KnowledgeConquest.Client.Connection;

namespace KnowledgeConquest.Client
{
    public class Conquest : MonoBehaviour
    {
        private WorldMap _worldMap;
        private QuestionPanel _questionPanel;
        private QuestionsRepository _questionsRepository;
        private MapApi _mapApi;

        private QuestionProcess _questionProcess;
        private Vector2Int _selectedCell;


        [Inject]
        public void Construct(WorldMap worldMap, QuestionPanel questionsPanel, QuestionsRepository questionsRepository, MapApi mapApi)
        {
            _worldMap = worldMap;
            _questionPanel = questionsPanel;
            _questionsRepository = questionsRepository;
            _mapApi = mapApi;
        }

        private void Update()
        {
            if (_questionProcess == null)
            {
                UpdateOnMap();
            }
        }

        private void UpdateOnMap()
        {
            var selectedCell = _worldMap.TryClickAvailiableCell();
            if (selectedCell.HasValue)
            {
                BeginQuestionAsync(selectedCell.Value);
            }
        }

        private async void BeginQuestionAsync(Vector2Int cell)
        {
            _selectedCell = cell;
            var question = await _questionsRepository.GetQuestionAsync(_selectedCell);
            _questionProcess = new QuestionProcess(cell, question, _mapApi);
            _questionProcess.OnAnswered += OnQuestionAnswered;
            _questionPanel.Open(_questionProcess);
        }

        private void OnQuestionAnswered(bool isCorrect)
        {
            _questionProcess = null;
            if (isCorrect)
            {
                _worldMap.SetCellOwned(_selectedCell);
            }
        }
    }
}
