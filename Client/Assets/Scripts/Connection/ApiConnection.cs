using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KnowledgeConquest.Client.Extensions;
using KnowledgeConquest.Client.Utils;

namespace KnowledgeConquest.Client.Connection
{
    public interface IApiConnection
    {
        Task<bool> IsServerAvailiableAsync();
        Task<T> GetAsync<T>(UrlBuilder url);
        Task<UnityWebRequest> GetAsync(string relativePath);
        Task<UnityWebRequest> GetAsync(UrlBuilder url);
        Task<UnityWebRequest> PostJsonAsync(string relativePath, JToken jsonToken);
        Task<UnityWebRequest> PostJsonAsync(UrlBuilder url, JToken jsonToken);
    }

    public sealed class ApiConnection : IApiConnection
    {
        private readonly IConnectionConfig _connectionConfig;

        public ApiConnection(IConnectionConfig connectionConfig)
        {
            _connectionConfig = connectionConfig;
        }

        public async Task<bool> IsServerAvailiableAsync()
        {
            using var request = UnityWebRequest.Head(_connectionConfig.BaseUrl);
            await request.SendWebRequest();
            return request.result != UnityWebRequest.Result.ConnectionError;
        }

        public async Task<T> GetAsync<T>(UrlBuilder url)
        {
            using var request = await GetAsync(url);
            if (request.result != UnityWebRequest.Result.Success)
            {
                ConnectionUtils.LogRequestError(request);
                return default;
            }
            return JObject.Parse(request.downloadHandler.text).ToObject<T>();
        }

        public Task<UnityWebRequest> GetAsync(string relativePath)
        {
            return GetAsync(UrlBuilder.FromRelative(relativePath));
        }

        public async Task<UnityWebRequest> GetAsync(UrlBuilder url)
        {
            url.BasePath ??= _connectionConfig.BaseUrl;
            var request = UnityWebRequest.Get(url.ToString());
            await request.SendWebRequest();
            return request;
        }

        public Task<UnityWebRequest> PostJsonAsync(string relativePath, JToken jsonToken)
        {
            return PostJsonAsync(UrlBuilder.FromRelative(relativePath), jsonToken);
        }

        public async Task<UnityWebRequest> PostJsonAsync(UrlBuilder url, JToken jsonToken)
        {
            url.BasePath ??= _connectionConfig.BaseUrl;
            var request = new UnityWebRequest(url.ToString(), UnityWebRequest.kHttpVerbPOST);
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonToken.WriteTo(jsonWriter);
                }
                var buffer = stream.GetBuffer();
                var len = System.Array.IndexOf<byte>(buffer, 0);
                if (len == -1)
                {
                    len = buffer.Length;
                }
                var dataToSend = new byte[len];
                System.Array.Copy(buffer, dataToSend, len);
                var uploadHandler = new UploadHandlerRaw(dataToSend);
                request.uploadHandler = uploadHandler;
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                ConnectionUtils.LogRequestError(request);
                return request;
            }
            return request;
        }
    }
}
