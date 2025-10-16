using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace MMS.Service.CheckDeposit.Repository.Common
{
    public class JsonContent : StringContent
    {
        private const string JsonType = "application/json";
        public JsonContent(string content) : base(content, System.Text.Encoding.UTF8, JsonType)
        {

        }

    }
}
