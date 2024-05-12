using UnityEngine;
using UnityEngine.Networking;

namespace KnowledgeConquest.Client.Connection
{
    public static class ConnectionUtils
    {
        public static void LogRequestError(UnityWebRequest request)
        {
            Debug.LogError($"{request.url}: {request.result}, {request.responseCode}\n{request.downloadHandler.text}");
        }
    }
}
