using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Models;


namespace Common.Client
{
    public interface IWebApiClient
    {
        HttpResponseMessage ApiPut<T>(string url, T request, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null);

        HttpResponseMessage ApiGet(string url, string apiUsername = null, string apiPassword = null,
            int timeOut = -1, Dictionary<string, string> requestLoggingValues = null);

        HttpResponseMessage ApiGet(string url, Dictionary<string, string> queries, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null);

        HttpResponseMessage ApiPost<T>(string url, T request, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null);

        HttpResponseMessage ApiPostMultiPart(string url, List<MultiPartEntity> entities, string boundary,
            string apiUsername = null, string apiPassword = null, int timeOut = -1,
            Dictionary<string, string> requestLoggingValues = null);

        HttpResponseMessage ApiDelete(string url, string apiUsername = null, string apiPassword = null,
            int timeOut = -1, Dictionary<string, string> requestLoggingValues = null);

        Task<HttpResponseMessage> ApiPostAsync<T>(string url, T request, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null);

        Task<HttpResponseMessage> ApiPostMultiPartAsync(string url, List<MultiPartEntity> entities,
            string boundary, string apiUsername = null, string apiPassword = null, int timeOut = -1,
            Dictionary<string, string> requestLoggingValues = null);

        Task<HttpResponseMessage> ApiPutAsync<T>(string url, T request, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null);

        Task<HttpResponseMessage> ApiGetAsync(string url, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null);

        Task<HttpResponseMessage> ApiGetAsync(string url, Dictionary<string, string> queries,
            string apiUsername = null, string apiPassword = null, int timeOut = -1,
            Dictionary<string, string> requestLoggingValues = null);

        Task<HttpResponseMessage> ApiDeleteAsync(string url, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null);

        T ConvertJsonToObject<T>(HttpResponseMessage httpResponse);

        Dictionary<string, string> GetExternalApiUrls();

    }
}
