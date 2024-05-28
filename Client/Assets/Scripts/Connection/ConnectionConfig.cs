using UnityEngine;

namespace KnowledgeConquest.Client.Connection
{
    [CreateAssetMenu(menuName = "Data/Connection Config")]
    public sealed class ConnectionConfig : ScriptableObject, IConnectionConfig
    {
        public string BaseUrl => _baseUrl;
        [SerializeField] private string _baseUrl = "http://localhost/";
    }
}
