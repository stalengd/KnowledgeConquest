using System.Collections.Generic;

namespace KnowledgeConquest.Client
{
    public sealed class WorldMap 
    {
        public UserMap PrimaryMap { get; set; } 
        public IEnumerable<UserMap> UserMaps => _userMaps.Values;

        public event System.Action<UserMap> UserMapAdded;


        private readonly Dictionary<User, UserMap> _userMaps = new();

        public UserMap GetOrCreateUserMap(User user)
        {
            if (!_userMaps.TryGetValue(user, out UserMap userMap))
            {
                userMap = new UserMap(user, this);
                user.Map = userMap;
                _userMaps.Add(user, userMap);
                UserMapAdded?.Invoke(userMap);
            }
            return userMap;
        }
    }
}