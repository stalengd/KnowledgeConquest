using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KnowledgeConquest.Client
{
    public class WorldMap : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TileBase _ownedTile;
        [SerializeField] private TileBase _freeTile;
        [SerializeField] private TileBase _availableTile;
        [SerializeField] private RectInt _visibleField;
        private HashSet<Vector2Int> _ownedCells;

        private void Start()
        {
            _ownedCells = new HashSet<Vector2Int>()
            {
                new Vector2Int(0, 0),
            };
            Draw();
        }

        public Vector2Int? TryClickAvailiableCell()
        {
            if (!Input.GetMouseButtonDown(0)) return null;
            var mouseCell = WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (!_visibleField.Contains(mouseCell)) return null;
            if (_ownedCells.Contains(mouseCell)) return null;
            if (!IsNeighbourOwned(mouseCell)) return null;

            return mouseCell;
        }

        public void SetCellOwned(Vector2Int cell, bool redraw = true)
        {
            var isAdded = _ownedCells.Add(cell);
            if (redraw && isAdded)
            {
                Draw();
            }
        }

        public void Draw()
        {
            _tilemap.ClearAllTiles();
            var pos = new Vector2Int(_visibleField.xMin, _visibleField.yMax);
            for (; pos.x < _visibleField.xMax; pos.x++)
            {
                for (; pos.y < _visibleField.yMax; pos.y++)
                {
                    TileBase tile = _freeTile;
                    if (_ownedCells.Contains(pos))
                        tile = _ownedTile;
                    else if (IsNeighbourOwned(pos))
                        tile = _availableTile;
                    _tilemap.SetTile((Vector3Int)pos, tile);
                }
                pos.y = _visibleField.yMin;
            }
        }

        private Vector2Int WorldToCell(Vector2 worldPos)
        {
            return (Vector2Int)_grid.WorldToCell(worldPos);
        }

        private bool IsNeighbourOwned(Vector2Int cell)
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