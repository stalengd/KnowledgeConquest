using UnityEngine;
using Zenject;
using KnowledgeConquest.Client.UI;
using KnowledgeConquest.Client.Connection;
using KnowledgeConquest.Client.Extensions;

namespace KnowledgeConquest.Client
{
    public class Conquest : MonoBehaviour
    {
        private WorldMap _worldMap;
        private WorldMapRenderer _worldMapRenderer;
        private QuestionPanel _questionPanel;
        private QuestionsRepository _questionsRepository;
        private MapApi _mapApi;
        private CameraController _cameraController;
        private LoadingIndicator _loadingIndicator;

        private QuestionProcess _questionProcess;
        private Vector2Int _selectedCell;
        private float _clickStartTime;


        [Inject]
        public void Construct(
            WorldMap worldMap,
            WorldMapRenderer worldMapRenderer,
            QuestionPanel questionsPanel,
            QuestionsRepository questionsRepository,
            MapApi mapApi,
            CameraController cameraController,
            LoadingIndicator loadingIndicator)
        {
            _worldMap = worldMap;
            _worldMapRenderer = worldMapRenderer;
            _questionPanel = questionsPanel;
            _questionsRepository = questionsRepository;
            _mapApi = mapApi;
            _cameraController = cameraController;
            _loadingIndicator = loadingIndicator;
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
            if (Input.GetKeyDown(KeyCode.Mouse0)) 
            {
                _clickStartTime = Time.realtimeSinceStartup;
            }
            if (Input.GetKeyUp(KeyCode.Mouse0)) 
            {
                var clickDuration = Time.realtimeSinceStartup - _clickStartTime;
                if (clickDuration > 0.25f) 
                {
                    return;
                }
                var selectedCell = _worldMapRenderer.TryGetAvailiableCell(Input.mousePosition);
                if (selectedCell.HasValue)
                {
                    BeginQuestionAsync(selectedCell.Value);
                }
            }
        }

        private async void BeginQuestionAsync(Vector2Int cell)
        {
            _selectedCell = cell;
            EnterQuestionState();
            _loadingIndicator.Show();
            var question = await _questionsRepository.GetQuestionAsync(_selectedCell);
            _loadingIndicator.Hide();
            _questionProcess = new QuestionProcess(cell, question, _mapApi);
            _questionProcess.OnAnswered += OnQuestionAnswered;
            _questionPanel.Open(_questionProcess);
        }

        private void OnQuestionAnswered(bool isCorrect)
        {
            ExitQuestionState();
            _questionProcess = null;
            if (isCorrect)
            {
                _worldMap.PrimaryMap.SetCell(_selectedCell, UserMap.CellState.Owned);
            }
        }

        private void EnterQuestionState()
        {
            _cameraController.IsControlsEnabled = false;
            _cameraController.MoveTo(_worldMapRenderer.CellToWorld(_selectedCell).ToXZPlane());
        }

        private void ExitQuestionState()
        {
            _cameraController.IsControlsEnabled = true;
        }
    }
}
