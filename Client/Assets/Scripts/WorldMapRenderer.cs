using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using KnowledgeConquest.Shared.Math;
using KnowledgeConquest.Client.Extensions;
using KnowledgeConquest.Client.UI;

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
        [SerializeField] private int _userIsleRadius = 4;
        [SerializeField] private GameObject _userInfoOverlayPrefab;
        [SerializeField] private Vector2 _boundsExtends = Vector2.one * 2f;

        private WorldMap _worldMap;
        private CameraController _cameraController;
        private int _nextUserIndex = 1;
        private int _usersSpiralRadius = 0;
        private readonly Dictionary<UserMap, UserMapState> _userMaps = new();
        private readonly List<CubeCoords> _usersSpiral = new() { CubeCoords.Zero };
        private readonly List<CubeCoords> _isleCoords = new();

        private struct UserMapState
        {
            public System.Action<Vector2Int> Listener { get; set; }
            public int Index { get; set; }
            public UserInfoOverlay Overlay { get; set; }
        }


        [Inject]
        public void Construct(WorldMap worldMap, CameraController cameraController)
        {
            _worldMap = worldMap;
            _cameraController = cameraController;
        }

        private void Start()
        {
            _tilemap.ClearAllTiles();
            HexMath.Spiral(CubeCoords.Zero, _userIsleRadius, _isleCoords);
            _worldMap.UserMapAdded += OnMapAdded;
            foreach (var map in _worldMap.UserMaps)
            {
                OnMapAdded(map);
                Draw(map);
            }
        }

        private void OnDestroy()
        {
            _worldMap.UserMapAdded -= OnMapAdded;
            foreach (var map in _worldMap.UserMaps)
            {
                map.CellChanged -= _userMaps[map].Listener;
            }
        }

        public Vector2Int? TryClickAvailiableCell()
        {
            if (!Input.GetMouseButtonDown(0)) return null;
            var mouseCell = WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (!_visibleField.Contains(mouseCell)) return null;
            if (_worldMap.PrimaryMap.IsCellOwned(mouseCell)) return null;
            if (!_worldMap.PrimaryMap.IsNeighbourOwned(mouseCell)) return null;

            return mouseCell;
        }

        private void Draw(UserMap userMap)
        {
            var state = _userMaps[userMap];
            var c = GetUserLocalCoords(state.Index);
            var cWorld = c * (_userIsleRadius * 2 + 1);
            var n = new CubeCoords
            {
                R = c.Q,
                S = c.R,
                Q = c.S
            };
            cWorld += n * _userIsleRadius;

            var mapBounds = new Bounds(CellToWorld(cWorld.ToOffsetCoords().AsVector2Int()), Vector3.zero);
            for (int i = 0; i < _isleCoords.Count; i++)
            {
                var tileCoord = cWorld + _isleCoords[i];
                var localCell = _isleCoords[i].ToOffsetCoords().AsVector2Int();
                var worldCell = tileCoord.ToOffsetCoords().AsVector2Int();
                TileBase tile = _freeTile;
                if (userMap.IsCellOwned(localCell))
                    tile = _ownedTile;
                else if (userMap.IsNeighbourOwned(localCell))
                    tile = _availableTile;
                _tilemap.SetTile((Vector3Int)worldCell, tile);
                mapBounds.Encapsulate(CellToWorld(worldCell));
            }
            mapBounds.Expand(_boundsExtends);

            var cameraRect = _cameraController.ViewBounds;
            var cameraBounds = new Bounds(cameraRect.center, cameraRect.size);
            cameraBounds.Encapsulate(mapBounds.min);
            cameraBounds.Encapsulate(mapBounds.max);
            _cameraController.ViewBounds = new Rect(cameraBounds.min, cameraBounds.size);

            if (!userMap.IsPrimary && state.Overlay == null)
            {
                var overlayWorldPos = CellToWorld(cWorld.ToOffsetCoords().AsVector2Int()); 
                state.Overlay = Instantiate(_userInfoOverlayPrefab, overlayWorldPos, Quaternion.identity)
                    .GetComponent<UserInfoOverlay>();
                state.Overlay.Render(userMap.Owner);
            }
        }

        private void OnMapAdded(UserMap map)
        {
            var mapState = new UserMapState()
            {
                Listener = _ => OnMapChanged(map),
                Index = map.IsPrimary ? 0 : _nextUserIndex++,
            };
            _userMaps.Add(map, mapState);
            map.CellChanged += mapState.Listener;
        }

        private void OnMapChanged(UserMap map)
        {
            Draw(map);
        }

        private Vector2Int WorldToCell(Vector2 worldPos)
        {
            return (Vector2Int)_grid.WorldToCell(worldPos);
        }

        private Vector2 CellToWorld(Vector2Int cell)
        {
            return (Vector2)_grid.CellToWorld((Vector3Int)cell);
        }

        private CubeCoords GetUserLocalCoords(int index)
        {
            if (_usersSpiral.Count <= index)
            {
                _usersSpiralRadius++;
                HexMath.Ring(CubeCoords.Zero, _usersSpiralRadius, _usersSpiral);
            }
            return _usersSpiral[index];
        }
    }
}