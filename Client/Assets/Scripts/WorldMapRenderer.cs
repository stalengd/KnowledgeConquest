using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace KnowledgeConquest.Client
{
    public sealed class WorldMapRenderer : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TileBase _ownedTile;
        [SerializeField] private TileBase _freeTile;
        [SerializeField] private TileBase _availableTile;
        [SerializeField] private RectInt _visibleField;

        private WorldMap _worldMap;


        [Inject]
        public void Construct(WorldMap worldMap)
        {
            _worldMap = worldMap;
        }

        private void Start()
        {
            _worldMap.CellChanged += OnCellChanged;
            Draw();
        }

        private void OnDestroy()
        {
            _worldMap.CellChanged -= OnCellChanged;
        }

        public Vector2Int? TryClickAvailiableCell()
        {
            if (!Input.GetMouseButtonDown(0)) return null;
            var mouseCell = WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (!_visibleField.Contains(mouseCell)) return null;
            if (_worldMap.IsCellOwned(mouseCell)) return null;
            if (!_worldMap.IsNeighbourOwned(mouseCell)) return null;

            return mouseCell;
        }

        private void Draw()
        {
            _tilemap.ClearAllTiles();
            var pos = new Vector2Int(_visibleField.xMin, _visibleField.yMax);
            for (; pos.x < _visibleField.xMax; pos.x++)
            {
                for (; pos.y < _visibleField.yMax; pos.y++)
                {
                    TileBase tile = _freeTile;
                    if (_worldMap.IsCellOwned(pos))
                        tile = _ownedTile;
                    else if (_worldMap.IsNeighbourOwned(pos))
                        tile = _availableTile;
                    _tilemap.SetTile((Vector3Int)pos, tile);
                }
                pos.y = _visibleField.yMin;
            }
        }

        private void OnCellChanged(Vector2Int cell)
        {
            Draw();
        }

        private Vector2Int WorldToCell(Vector2 worldPos)
        {
            return (Vector2Int)_grid.WorldToCell(worldPos);
        }
    }
}