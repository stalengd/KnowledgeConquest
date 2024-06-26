using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using KnowledgeConquest.Client.Extensions;
using KnowledgeConquest.Shared.Math;

namespace KnowledgeConquest.Client.DebugTools
{
    public sealed class DummyMap : MonoBehaviour
    {
        [SerializeField] private int _capturedSuccessfullyCount = 5;
        [SerializeField] private int _capturedFailyCount = 5;
        private WorldMap _worldMap;

        [Inject]
        public void Construct(WorldMap worldMap)
        {
            _worldMap = worldMap;
        }

        private void Start()
        {
            if (!_worldMap.UserMaps.Any())
            {
                var map = _worldMap.GetOrCreateUserMap(new User("dummy_user"));
                _worldMap.PrimaryMap = map;
                var cells = new List<CubeCoords>();
                HexMath.Spiral(CubeCoords.Zero, 3, cells);
                for (int i = 0; i < cells.Count; i++)
                {
                    var cell = cells[i].ToOffsetCoords().AsVector2Int();
                    if (i < _capturedSuccessfullyCount)
                    {
                        map.SetCell(cell, UserMap.CellState.CapturedSuccessfuly);
                    }
                    else if (i < _capturedSuccessfullyCount + _capturedFailyCount)
                    {
                        map.SetCell(cell, UserMap.CellState.CapturedFaily);
                    }
                }
            }
        }
    }
}
