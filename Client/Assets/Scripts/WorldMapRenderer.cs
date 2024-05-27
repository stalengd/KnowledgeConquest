using System.Collections.Generic;
using UnityEngine;
using Zenject;
using KnowledgeConquest.Shared.Math;
using KnowledgeConquest.Client.Extensions;
using KnowledgeConquest.Client.UI;

namespace KnowledgeConquest.Client
{
    public sealed class WorldMapRenderer : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private GameObject _tilePrefabOwned;
        [SerializeField] private GameObject _tilePrefabAvailiable;
        [SerializeField] private int _userIsleRadius = 4;
        [SerializeField] private GameObject _userInfoOverlayPrefab;
        [SerializeField] private Vector2 _boundsExtends = Vector2.one * 2f;
        [SerializeField] private float _randomHeightMagnitude = 0.1f;

        private WorldMap _worldMap;
        private CameraController _cameraController;
        private int _nextUserIndex = 1;
        private int _usersSpiralRadius = 0;
        private readonly Dictionary<UserMap, UserMapState> _userMaps = new();
        private readonly List<CubeCoords> _usersSpiral = new() { CubeCoords.Zero };
        private readonly List<CubeCoords> _isleCoords = new();
        private readonly Dictionary<Vector2Int, GameObject> _tilemap = new();


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
            if (!Input.GetMouseButtonDown(0)) 
            {
                return null;
            }
            var plane = new Plane(transform.up, transform.position);
            var ray = _cameraController.Camera.ScreenPointToRay(Input.mousePosition);
            if (!plane.Raycast(ray, out var hitDistance))
            {
                return null;
            }
            var mouseCell = WorldToCell(ray.GetPoint(hitDistance));
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
                if (userMap.IsCellOwned(localCell))
                {
                    SetTile(worldCell, _tilePrefabOwned);
                }
                else if (userMap.IsPrimary && userMap.IsNeighbourOwned(localCell))
                {
                    SetTile(worldCell, _tilePrefabAvailiable);
                }
                else 
                {
                    SetTile(worldCell, null);
                }
                mapBounds.Encapsulate(CellToWorld(worldCell));
            }
            mapBounds.Expand(_boundsExtends);

            var cameraRect = _cameraController.ViewBounds;
            var cameraBounds = new Bounds(cameraRect.center.FromXZPlane(), cameraRect.size.FromXZPlane());
            cameraBounds.Encapsulate(mapBounds.min);
            cameraBounds.Encapsulate(mapBounds.max);
            _cameraController.ViewBounds = new Rect(cameraBounds.min.ToXZPlane(), cameraBounds.size.ToXZPlane());

            if (!userMap.IsPrimary && state.Overlay == null)
            {
                var overlayWorldPos = CellToWorld(cWorld.ToOffsetCoords().AsVector2Int()); 
                state.Overlay = Instantiate(_userInfoOverlayPrefab, overlayWorldPos, Quaternion.identity)
                    .GetComponent<UserInfoOverlay>();
                state.Overlay.Render(userMap.Owner);
            }
        }

        private void SetTile(Vector2Int cell, GameObject prefab)
        {
            var cellWorldPos = CellToWorld(cell);
            cellWorldPos.y += Mathf.PerlinNoise(cellWorldPos.x, cellWorldPos.z) * _randomHeightMagnitude;

            if (!_tilemap.TryGetValue(cell, out var tile))
            {
                if (prefab == null)
                {
                    return;
                }
                _tilemap.Add(cell, Instantiate(prefab, cellWorldPos, Quaternion.identity, transform));
                return;
            }

            Destroy(tile);
            _tilemap.Remove(cell);

            if (prefab == null)
            {
                return;
            }

            _tilemap[cell] = Instantiate(prefab, cellWorldPos, Quaternion.identity, transform);
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

        private Vector2Int WorldToCell(Vector3 worldPos)
        {
            return (Vector2Int)_grid.WorldToCell(worldPos);
        }

        private Vector3 CellToWorld(Vector2Int cell)
        {
            return _grid.CellToWorld(new Vector3Int(cell.x, cell.y));
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