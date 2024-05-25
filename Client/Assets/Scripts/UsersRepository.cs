using System.Collections.Generic;

namespace KnowledgeConquest.Client
{
    public interface IUsersRepository
    {
        IReadOnlyCollection<User> Users { get; }
        User GetOrCreateUser(string userId);
    }

    public sealed class UsersRepository : IUsersRepository
    {
        public IReadOnlyCollection<User> Users => _users.Values;

        private readonly Dictionary<string, User> _users = new();

        public User GetOrCreateUser(string userId)
        {
            if (!_users.TryGetValue(userId, out var user))
            {
                user = new User(userId);
                _users.Add(userId, user);
                return user;
            }
            return user;
        }
    }
}
