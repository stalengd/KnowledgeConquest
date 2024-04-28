using UnityEngine;

namespace KnowledgeConquest.Client.Connection
{
    [CreateAssetMenu(menuName = "Data/Connection Config")]
    public sealed class ConnectionConfig : ScriptableObject, IConnectionConfig
    {
        public System.Uri BaseUrl => new(_baseUrl);
        [SerializeField] private string _baseUrl = new("http://localhost/");

        public string Username => _username;
        [SerializeField] private string _username = "user";

        public string Password => _password;
        [SerializeField] private string _password = "123";
    }
}
