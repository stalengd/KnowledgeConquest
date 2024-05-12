﻿using System.Collections.Generic;
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

        public async Task<List<Validation.Error>> RegisterAsync(string username, string password)
        {
            var data = new JObject() 
            {
                ["username"] = username,
                ["password"] = password,
            };
            using var request = await _apiConnection.PostJsonAsync("Account/Register", data);
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
