using System.Collections.Generic;
using UnityEngine;

namespace KnowledgeConquest.Client
{
    public sealed class UserMap 
    {
        public User Owner { get; }
        public bool IsPrimary => _worldMap.PrimaryMap == this;

        public event System.Action<Vector2Int> CellChanged;

        private readonly HashSet<Vector2Int> _ownedCells = new()
        {
            new Vector2Int(0, 0),
        };
        private readonly WorldMap _worldMap;

        public UserMap(User owner, WorldMap worldMap)
        {
            Owner = owner;
            _worldMap = worldMap;
        }

        public void SetCellOwned(Vector2Int cell, bool notify = true)
        {
            var isAdded = _ownedCells.Add(cell);
            if (notify && isAdded)
            {
                CellChanged?.Invoke(cell);
            }
        }

        public bool IsCellOwned(Vector2Int cell)
        {
            return _ownedCells.Contains(cell);
        }

        public bool IsNeighbourOwned(Vector2Int cell)
        {
            if (cell.y % 2 == 0) 
            {
                return _ownedCells.Contains(cell + new Vector2Int(1, 0)) ||
                    _ownedCells.Contains(cell + new Vector2Int(-1, 0)) ||
                    _ownedCells.Contains(cell + new Vector2Int(0, 1)) ||
                    _ownedCells.Contains(cell + new Vector2Int(0, -1)) ||
                    _ownedCells.Contains(cell + new Vector2Int(-1, 1)) ||
                    _ownedCells.Contains(cell + new Vector2Int(-1, -1));
            }
            else
            {
                return _ownedCells.Contains(cell + new Vector2Int(1, 0)) ||
                    _ownedCells.Contains(cell + new Vector2Int(-1, 0)) ||
                    _ownedCells.Contains(cell + new Vector2Int(0, 1)) ||
                    _ownedCells.Contains(cell + new Vector2Int(0, -1)) ||
                    _ownedCells.Contains(cell + new Vector2Int(1, 1)) ||
                    _ownedCells.Contains(cell + new Vector2Int(1, -1));
            }
        }
    }
}