using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Common.Client;

namespace MMS.Service.CheckDeposit.Repository.Common
{   
    public class SoapClient : ISoapClient
    {
        #region Member Variables

       
        private HttpClient _httpClient;
        private bool? _isLinux;

        public bool IsLinuxPlatform
        {
            get => _isLinux ?? RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            set => _isLinux = value;
        }

        #endregion Member Variables

        #region Constructors

        public SoapClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #endregion Constructors

        public string ApiPost<T>(string url, T request, string apiUsername = null, string apiPassword = null,
            int timeOut = -1, Dictionary<string, string> requestLoggingValues = null)
        {
            var xml = ToXML(request);            
            var formattedXml = string.Empty; //xml.Remove(0, 41);
            if (IsLinuxPlatform)
            {
                formattedXml = xml.Remove(0, 40);
            }
            else
            {
                formattedXml = xml.Remove(0, 41);
            }
            var soapRequest = GetSoapRequest(formattedXml);

            var content = new StringContent(soapRequest, Encoding.UTF8, "application/soap+xml");

            InitHttpClient(_httpClient, apiUsername, apiPassword, requestLoggingValues);
            try
            {
                var postAsyncResponse = _httpClient.PostAsync(url, content);
                postAsyncResponse.Wait();
                var response = postAsyncResponse.Result;
                var stringContent = response.Content.ReadAsStringAsync().Result;
                return stringContent;
            }
            catch (AggregateException ex)
            {
                //if (ex.InnerExceptions.Any(e => e is TaskCanceledException))
                if (ex.Flatten().InnerExceptions.Any(e => e is TaskCanceledException))
                {
                    throw new TimeoutException(ex.Message, ex);
                }

                throw new Exception(ex.Message, ex);
            }
      
        }

        public T Deserialize<T>(string xmlStr)
        {
            var serializer = new XmlSerializer(typeof(T));
            T result;
            var xDoc = XDocument.Load(new StringReader(xmlStr));

            var soapBody = xDoc.Descendants().First(p => p.Name.LocalName == "Body").FirstNode;

            using (TextReader reader = new StringReader(soapBody.ToString()))
            {
                result = (T)serializer.Deserialize(reader);
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


        #region HelperMethods

        private static string ToXML(object obj)
        {
            var serializer = new XmlSerializer(obj.GetType(), "http://validationservice.vsoftcorp.com/");

            using var stringWriter = new System.IO.StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true });
            serializer.Serialize(xmlWriter, obj);
            return stringWriter.ToString();
        }

        private static string GetSoapRequest(string requestBody)
        {
            var xmlStr = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                      xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                      xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
                      <soap12:Body>
                        {0}
                      </soap12:Body>
                    </soap12:Envelope>";

            var s = String.Format(xmlStr, requestBody);
            return s;

        }
        #endregion
    }
}
