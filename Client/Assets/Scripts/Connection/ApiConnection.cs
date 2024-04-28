using System.Threading.Tasks;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KnowledgeConquest.Client.Extensions;

namespace KnowledgeConquest.Client.Connection
{
    public sealed class ApiConnection
    {
        private readonly IConnectionConfig _connectionConfig;

        public ApiConnection(IConnectionConfig connectionConfig)
        {
            _connectionConfig = connectionConfig;
        }

        public async ValueTask<bool> IsServerAvailiableAsync()
        {
            using var request = UnityWebRequest.Head(_connectionConfig.BaseUrl);
            await request.SendWebRequest();
            Debug.Log($"Server availiability: {request.result}");
            return request.result != UnityWebRequest.Result.ConnectionError;
        }

        public async ValueTask<bool> RegisterAsync()
        {
            var data = new JObject() 
            {
                ["username"] = _connectionConfig.Username,
                ["password"] = _connectionConfig.Password,
            };
            using var request = PostJson("Account/Register", data);
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                LogRequestError(request);
                return false;
            }
            Debug.Log(request.downloadHandler.text);
            return true;
        }

        public async ValueTask<bool> LoginAsync()
        {
            var data = new JObject() 
            {
                ["username"] = _connectionConfig.Username,
                ["password"] = _connectionConfig.Password,
                ["remember"] = true,
            };
            using var request = PostJson("Account/Login", data);
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                LogRequestError(request);
                return false;
            }
            Debug.Log(request.downloadHandler.text);
            return true;
        }


        private void LogRequestError(UnityWebRequest request)
        {
            Debug.LogError($"{request.url}: {request.result}, {request.responseCode}\n{request.downloadHandler.text}");
        }

        private UnityWebRequest Get(string relativePath)
        {
            return UnityWebRequest.Get(new System.Uri(_connectionConfig.BaseUrl, relativePath));
        }

        private UnityWebRequest PostJson(string relativePath, JToken jsonToken)
        {
            var request = new UnityWebRequest(new System.Uri(_connectionConfig.BaseUrl, relativePath), UnityWebRequest.kHttpVerbPOST);
            jsonToken.CreateReader();

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonToken.WriteTo(jsonWriter);
                }
                var buffer = stream.GetBuffer();
                var nativeBuffer = new NativeArray<byte>(buffer, Allocator.Temp);
                var len = System.Array.IndexOf<byte>(buffer, 0);
                if (len == -1)
                {
                    len = buffer.Length;
                }
                var uploadHandler = new UploadHandlerRaw(nativeBuffer.GetSubArray(0, len), true);
                request.uploadHandler = uploadHandler;
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }
    }
}
