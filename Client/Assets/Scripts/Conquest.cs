using UnityEngine;
using Zenject;
using KnowledgeConquest.Client.UI;

namespace KnowledgeConquest.Client
{
    public class Conquest : MonoBehaviour
    {
        private WorldMap _worldMap;
        private QuestionPanel _questionPanel;
        private QuestionsRepository _questionsRepository;

        private QuestionProcess _questionProcess;
        private Vector2Int _selectedCell;


        [Inject]
        public void Construct(WorldMap worldMap, QuestionPanel questionsPanel, QuestionsRepository questionsRepository)
        {
            _worldMap = worldMap;
            _questionPanel = questionsPanel;
            _questionsRepository = questionsRepository;
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
                BeginQuestion(selectedCell.Value);
            }
        }

        private void BeginQuestion(Vector2Int cell)
        {
            _selectedCell = cell;
            var question = _questionsRepository.GetQuestion(_selectedCell);
            _questionProcess = new QuestionProcess(question);
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
