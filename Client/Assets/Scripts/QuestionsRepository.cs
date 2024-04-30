using System.Threading.Tasks;
using UnityEngine;
using KnowledgeConquest.Client.Models;
using KnowledgeConquest.Client.Connection;

namespace KnowledgeConquest.Client
{
    public class QuestionsRepository 
    {
        private readonly MapApi _mapApi;

        public QuestionsRepository(MapApi mapApi)
        {
            _mapApi = mapApi;
        }

        public Task<Question> GetQuestionAsync(Vector2Int cell)
        {
            return _mapApi.GetMapCellQuestionAsync(cell);
        }
    }
}
