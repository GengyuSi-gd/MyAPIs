namespace Common.Client
{
    public interface ISoapClient
    {
        string ApiPost<T>(string url, T request, string apiUsername = null,
            string apiPassword = null, int timeOut = -1, Dictionary<string, string> requestLoggingValues = null);

        T Deserialize<T>(string xmlStr);
        bool IsLinuxPlatform { get; set; }
    }
}
