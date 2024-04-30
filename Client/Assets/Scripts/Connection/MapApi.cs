using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json.Linq;
using KnowledgeConquest.Client.Utils;
using KnowledgeConquest.Client.Models;

namespace KnowledgeConquest.Client.Connection
{
    public sealed class MapApi
    {
        private readonly IApiConnection _apiConnection;

        public MapApi(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection;
        }

        public Task<UserMap> GetMapAsync()
        {
            return _apiConnection.GetAsync<UserMap>(UrlBuilder.FromRelative("Map"));
        }

        public Task<Question> GetMapCellQuestionAsync(Vector2Int cellPosition)
        {
            return _apiConnection.GetAsync<Question>(UrlBuilder.FromRelative("Map/CellQuestion")
                .Param("cellX", cellPosition.x.ToString())
                .Param("cellY", cellPosition.y.ToString()));
        }

        public async Task<bool> AnswerQuestionAsync(Vector2Int cellPosition, int[] answers)
        {
            var data = new JObject()
            {
                ["cellPosition"] = new JObject()
                {
                    ["x"] = cellPosition.x,
                    ["y"] = cellPosition.y,
                },
                ["answers"] = JArray.FromObject(answers),
            };
            using var request = await _apiConnection.PostJsonAsync("Map/AnswerCellQuestion", data);
            return (bool)JToken.Parse(request.downloadHandler.text);
        }
    }
}
