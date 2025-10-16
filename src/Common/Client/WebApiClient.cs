using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common.Client;
using Common.Models;
using Newtonsoft.Json;

namespace MMS.Service.CheckDeposit.Repository.Common
{
    public class WebApiClient : IWebApiClient
    {
        #region Member Variables

        
        private HttpClient _httpClient;

        #endregion Member Variables

        #region Constructors

        public WebApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #endregion Constructors

        #region Api Methods

        public HttpResponseMessage ApiPut<T>(string url, T request, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null)
        {
            StringContent content = new JsonContent(JsonConvert.SerializeObject(request));
            InitHttpClient(_httpClient, apiUsername, apiPassword, requestLoggingValues);


            try
            {
                return _httpClient.PutAsync(url, content).Result;
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }

        }

        public HttpResponseMessage ApiGet(string url, string apiUsername = null, string apiPassword = null,
            int timeOut = -1, Dictionary<string, string> requestLoggingValues = null)
        {

            InitHttpClient(_httpClient, apiUsername, apiPassword, requestLoggingValues);
            try
            {
                return _httpClient.GetAsync(url).Result;
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }


        }

        public HttpResponseMessage ApiGet(string url, Dictionary<string, string> queries, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null)
        {
            if (queries != null && queries.Count > 0)
            {
                var sb = new StringBuilder();

                if (!url.EndsWith("?"))
                {
                    sb.Append("?");
                }

                foreach (var q in queries)
                {
                    sb.Append(HttpUtility.UrlEncode(q.Key));
                    sb.Append("=");
                    sb.Append(HttpUtility.UrlEncode(q.Value));
                    sb.Append("&");
                }

                url += sb.ToString();
            }

            return ApiGet(url, apiUsername, apiPassword, timeOut, requestLoggingValues);
        }

        public HttpResponseMessage ApiPost<T>(string url, T request, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null)
        {
            StringContent content = new JsonContent(JsonConvert.SerializeObject(request));

            InitHttpClient(_httpClient, apiUsername, apiPassword, requestLoggingValues);
            try
            {
                return _httpClient.PostAsync(url, content).Result;
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }

        }

        public HttpResponseMessage ApiPostMultiPart(string url, List<MultiPartEntity> entities, string boundary,
            string apiUsername = null, string apiPassword = null, int timeOut = -1,
            Dictionary<string, string> requestLoggingValues = null)
        {

            InitHttpClient(_httpClient, apiUsername, apiPassword, requestLoggingValues);

            using (var content = new MultipartFormDataContent(boundary))
            {
                foreach (var multiPartEntity in entities)
                {
                    if (string.IsNullOrWhiteSpace(multiPartEntity.FileName))
                    {
                        content.Add(multiPartEntity.Content, multiPartEntity.Name);
                    }
                    else
                    {
                        content.Add(multiPartEntity.Content, multiPartEntity.Name, multiPartEntity.FileName);
                    }
                }


                try
                {
                    return _httpClient.PostAsync(url, content).Result;
                }
                catch (TaskCanceledException)
                {
                    throw new TimeoutException();
                }

            }

        }

        public HttpResponseMessage ApiDelete(string url, string apiUsername = null, string apiPassword = null,
            int timeOut = -1, Dictionary<string, string> requestLoggingValues = null)
        {

            InitHttpClient(_httpClient, apiUsername, apiPassword, requestLoggingValues);


            try
            {
                return _httpClient.DeleteAsync(url).Result;
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }


        }


        #endregion Api Methods

        #region Async API Methods

        public async Task<HttpResponseMessage> ApiPostAsync<T>(string url, T request, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null)
        {
            StringContent content = new JsonContent(JsonConvert.SerializeObject(request));


            InitHttpClient(_httpClient, apiUsername, apiPassword, requestLoggingValues);


            try
            {
                return await _httpClient.PostAsync(url, content);
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }
        }

        public async Task<HttpResponseMessage> ApiPostMultiPartAsync(string url, List<MultiPartEntity> entities,
            string boundary, string apiUsername = null, string apiPassword = null, int timeOut = -1,
            Dictionary<string, string> requestLoggingValues = null)
        {

            InitHttpClient(_httpClient, apiUsername, apiPassword, requestLoggingValues);

            using (var content = new MultipartFormDataContent(boundary))
            {
                foreach (var multiPartEntity in entities)
                {
                    if (string.IsNullOrWhiteSpace(multiPartEntity.FileName))
                    {
                        content.Add(multiPartEntity.Content, multiPartEntity.Name);
                    }
                    else
                    {
                        content.Add(multiPartEntity.Content, multiPartEntity.Name, multiPartEntity.FileName);
                    }
                }


                try
                {
                    return await _httpClient.PostAsync(url, content);
                }
                catch (TaskCanceledException)
                {
                    throw new TimeoutException();
                }

            }

        }

        public async Task<HttpResponseMessage> ApiPutAsync<T>(string url, T request, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null)
        {
            StringContent content = new JsonContent(JsonConvert.SerializeObject(request));


            InitHttpClient(_httpClient, apiUsername, apiPassword, requestLoggingValues);


            try
            {
                return await _httpClient.PutAsync(url, content);
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }

        }

        public async Task<HttpResponseMessage> ApiGetAsync(string url, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null)
        {

            InitHttpClient(_httpClient, apiUsername, apiPassword, requestLoggingValues);


            try
            {
                return await _httpClient.GetAsync(url);
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }

        }

        public async Task<HttpResponseMessage> ApiGetAsync(string url, Dictionary<string, string> queries,
            string apiUsername = null, string apiPassword = null, int timeOut = -1,
            Dictionary<string, string> requestLoggingValues = null)
        {
            if (queries != null && queries.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                if (!url.EndsWith("?"))
                {
                    sb.Append("?");
                }

                foreach (var q in queries)
                {
                    sb.Append(HttpUtility.UrlEncode(q.Key));
                    sb.Append("=");
                    sb.Append(HttpUtility.UrlEncode(q.Value));
                    sb.Append("&");
                }

                url += sb.ToString();
            }

            return await ApiGetAsync(url, apiUsername, apiPassword, timeOut, requestLoggingValues);
        }

        public async Task<HttpResponseMessage> ApiDeleteAsync(string url, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null)
        {
            
                InitHttpClient(_httpClient, apiUsername, apiPassword, requestLoggingValues);

              
                    try
                    {
                        return await _httpClient.DeleteAsync(url);
                    }
                    catch (TaskCanceledException)
                    {
                        throw new TimeoutException();
                    }
               
        }

        #endregion  Async API Methods

        #region Auxiliary Methods

        public T ConvertJsonToObject<T>(HttpResponseMessage httpResponse)
        {
            var stringContent = httpResponse.Content.ReadAsStringAsync().Result;
            T response;

            try
            {
                response = JsonConvert.DeserializeObject<T>(stringContent);

                return response;
            }
            catch 
            {
                response = default(T);
            }

            return response;
        }

        /// <summary>
        /// Read External API url's into a dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetExternalApiUrls()
        {
            return MemoryCacheUtility.AddOrGetExistingFromCache("ExternalApis", () =>
            {
                var externalApis = new Dictionary<string, string>();

                var config = JsonConvert.DeserializeObject<ServiceEndPoints>(File.ReadAllText(
                    $"{AppDomain.CurrentDomain.BaseDirectory}\\config\\externalApis.json"));

                if (config != null)
                {
                    foreach (ServiceEndPoint endpt in config.ExternalApis)
                    {
                        // Protect against inserting duplicate entries.  If entry already exists, skip.
                        if (!externalApis.ContainsKey(endpt.API))
                        {
                            externalApis.Add(endpt.API, endpt.URL);
                        }
                    }
                }
                return externalApis;
            });
        }

        private static string EncodeCredentials(string apiUsername, string apiPassword)
        {
            string result;

            if (!string.IsNullOrEmpty(apiUsername) && !string.IsNullOrEmpty(apiPassword))
            {
                result = Convert.ToBase64String(Encoding.ASCII.GetBytes(apiUsername + ":" + apiPassword));
            }
            else
            {
                result = string.Empty;
            }

            return result;
        }

        private static void InitHttpClient(HttpClient client, string apiUsername, string apiPassword,
            Dictionary<string, string> requestLoggingValues = null)
        {
            var credentials = EncodeCredentials(apiUsername, apiPassword);

            if (!string.IsNullOrWhiteSpace(credentials))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            }
            
            if (requestLoggingValues != null)
            {
                foreach (KeyValuePair<string, string> e in requestLoggingValues)
                {
                    client.DefaultRequestHeaders.Add(e.Key, e.Value);
                }
            }
        }

        #endregion Auxiliary Methods

    }
}
