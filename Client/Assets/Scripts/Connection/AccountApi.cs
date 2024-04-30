using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KnowledgeConquest.Client.Connection
{
    public sealed class AccountApi
    {
        private readonly IConnectionConfig _connectionConfig;
        private readonly IApiConnection _apiConnection;

        public AccountApi(IConnectionConfig connectionConfig, IApiConnection apiConnection)
        {
            _connectionConfig = connectionConfig;
            _apiConnection = apiConnection;
        }

        public async Task<bool> RegisterAsync()
        {
            var data = new JObject() 
            {
                ["username"] = _connectionConfig.Username,
                ["password"] = _connectionConfig.Password,
            };
            using var request = await _apiConnection.PostJsonAsync("Account/Register", data);
            return true;
        }

        public async Task<bool> LoginAsync()
        {
            var data = new JObject() 
            {
                ["username"] = _connectionConfig.Username,
                ["password"] = _connectionConfig.Password,
                ["remember"] = true,
            };
            using var request = await _apiConnection.PostJsonAsync("Account/Login", data);
            return true;
        }
    }
}
