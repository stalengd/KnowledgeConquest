using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KnowledgeConquest.Client.Connection
{
    public sealed class AccountApi
    {
        private readonly IApiConnection _apiConnection;

        public AccountApi(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection;
        }

        public async Task<List<Validation.Error>> RegisterAsync(string username, string password, string firstname, string surname)
        {
            var data = new JObject() 
            {
                ["username"] = username,
                ["password"] = password,
                ["firstname"] = firstname,
                ["surname"] = surname,
            };
            using var request = await _apiConnection.PostJsonAsync("Account/Register", data);
            if (request.responseCode == 200)
            {
                return new List<Validation.Error>();
            }
            return Validation.ParseErrors(JToken.Parse(request.downloadHandler.text));
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var data = new JObject() 
            {
                ["username"] = username,
                ["password"] = password,
                ["remember"] = true,
            };
            using var request = await _apiConnection.PostJsonAsync("Account/Login", data);
            return request.result == UnityEngine.Networking.UnityWebRequest.Result.Success;
        }
    }
}
