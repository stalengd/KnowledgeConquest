using System.Collections.Generic;
using UnityEngine;

namespace KnowledgeConquest.Client
{
    public sealed class UserMap 
    {
        public User Owner { get; }
        public bool IsPrimary => _worldMap.PrimaryMap == this;

        public event System.Action<Vector2Int> CellChanged;

        private readonly Dictionary<Vector2Int, CellState> _cells = new();
        private readonly WorldMap _worldMap;

        [System.Flags]
        public enum CellState
        {
            Free = 0,
            CapturedSuccessfuly = 1,
            CapturedFaily = 2,
            Captured = CapturedSuccessfuly | CapturedFaily,
        }

        public UserMap(User owner, WorldMap worldMap)
        {
            Owner = owner;
            _worldMap = worldMap;
        }

        public void SetCell(Vector2Int cell, CellState state, bool notify = true)
        {
            bool isChanged = false;
            if (!_cells.TryGetValue(cell, out var currentState))
            {
                _cells.Add(cell, state);
                isChanged = true;
            }
            else if (currentState != state)
            {
                _cells[cell] = state;
                isChanged = true;
            }
            if (notify && isChanged)
            {
                CellChanged?.Invoke(cell);
            }
        }

        public bool ContainsCell(Vector2Int cell)
        {
            return _cells.ContainsKey(cell);
        }

        public CellState? GetCellOrDefault(Vector2Int cell)
        {
            return _cells.TryGetValue(cell, out var state) ? state : null;
        }

        public bool IsCellOwned(Vector2Int cell)
        {
            return _cells.TryGetValue(cell, out var state) && (state & CellState.Captured) > 0;
        }

        public bool IsNeighbourOwned(Vector2Int cell)
        {
            if (cell.y % 2 == 0) 
            {
                return IsCellOwned(cell + new Vector2Int(1, 0)) ||
                    IsCellOwned(cell + new Vector2Int(-1, 0)) ||
                    IsCellOwned(cell + new Vector2Int(0, 1)) ||
                    IsCellOwned(cell + new Vector2Int(0, -1)) ||
                    IsCellOwned(cell + new Vector2Int(-1, 1)) ||
                    IsCellOwned(cell + new Vector2Int(-1, -1));
            }
            else
            {
                return IsCellOwned(cell + new Vector2Int(1, 0)) ||
                    IsCellOwned(cell + new Vector2Int(-1, 0)) ||
                    IsCellOwned(cell + new Vector2Int(0, 1)) ||
                    IsCellOwned(cell + new Vector2Int(0, -1)) ||
                    IsCellOwned(cell + new Vector2Int(1, 1)) ||
                    IsCellOwned(cell + new Vector2Int(1, -1));
            }
        }
    }
}