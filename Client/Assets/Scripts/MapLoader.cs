using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Zenject;
using KnowledgeConquest.Client.Connection;
using KnowledgeConquest.Client.Models;

namespace KnowledgeConquest.Client
{
    public interface IMapLoader
    {
        Task StartAsync();
    }

    public sealed class MapLoader : IMapLoader, ITickable
    {
        private readonly float _refreshInterval = 10f;
        private readonly WorldMap _worldMap;
        private readonly MapApi _mapApi;
        private readonly IUsersRepository _usersRepository;

        private bool _isUpdateRunning = false;
        private float _refreshTimer = 0f;

        public MapLoader(WorldMap worldMap, MapApi mapApi, IUsersRepository usersRepository)
        {
            _worldMap = worldMap;
            _mapApi = mapApi;
            _usersRepository = usersRepository;
        }

        public void Tick()
        {
            if (!_isUpdateRunning) return;
            _refreshTimer -= Time.deltaTime;
            if (_refreshTimer < 0f)
            {
                _refreshTimer = _refreshInterval;
                UpdateNeighbourMapsAsync();
            }
        }

        public async Task StartAsync()
        {
            var myMapTask = _mapApi.GetMapAsync();
            var othersMapsTask = _mapApi.GetNeighbourMapsAsync(); 
            var tasks = new List<Task>
            {
                myMapTask,
                othersMapsTask,
            };
            await Task.WhenAll(tasks);
            var primaryMap = UpdateMapFromData(myMapTask.Result);
            _worldMap.PrimaryMap = primaryMap;
            foreach (var map in othersMapsTask.Result)
            {
                UpdateMapFromData(map);
            }
            var userInfoTasks = _usersRepository.Users.Select(x => _mapApi.GetUserInfoAsync(x.Id)).ToArray();
            var usersInfo = await Task.WhenAll(userInfoTasks);
            foreach (var userInfo in usersInfo)
            {
                var user = _usersRepository.GetOrCreateUser(userInfo.Id);
                user.Username = userInfo.Username;
                user.Firstname = userInfo.Firstname;
                user.Surname = userInfo.Surname;
            }
            _isUpdateRunning = true;
            _refreshTimer = _refreshInterval;
        }

        private async void UpdateNeighbourMapsAsync()
        {
            var maps = await _mapApi.GetNeighbourMapsAsync(); 
            foreach (var map in maps)
            {
                UpdateMapFromData(map);
            }
        }

        private UserMap UpdateMapFromData(UserMapDTO data)
        {
            var user = _usersRepository.GetOrCreateUser(data.UserId);
            var map = _worldMap.GetOrCreateUserMap(user);
            foreach (var cell in data.Cells)
            {
                var state = cell.Type switch
                {
                    UserMapCellType.Free => UserMap.CellState.Free,
                    UserMapCellType.CapturedSuccessfuly => UserMap.CellState.CapturedSuccessfuly,
                    UserMapCellType.CapturedFaily => UserMap.CellState.CapturedFaily,
                    _ => UserMap.CellState.Free,
                };
                map.SetCell(new(cell.PositionX, cell.PositionY), state);
            }
            return map;
        }
    }
}
