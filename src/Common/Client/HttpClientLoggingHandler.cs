using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Common.Log;
using Microsoft.Extensions.Logging;

namespace MMS.Service.CheckDeposit.Repository.Common
{
    public class HttpClientLoggingHandler : DelegatingHandler
    {
        #region Constants

        private const string HttpErrorReason500 = "Internal Server Error";
        private const string HttpErrorReason405 = "Method Not Allowed";
        private const string HttpErrorReason400 = "400 Bad Request";
        private const string MaskingRestrictedDataEnabled = "MaskingRestrictedDataEnabled";

        #endregion Constants

        #region Member Variables

        private readonly ILogger<HttpClientLoggingHandler> _logRequestResponseRepository;

        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        #endregion Member Variables

        #region Constructors

        public HttpClientLoggingHandler(ILogger<HttpClientLoggingHandler> logRequestResponseRepository,
            IBackgroundTaskQueue backgroundTaskQueue)
        {
            _logRequestResponseRepository = logRequestResponseRepository;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        #endregion Constructors

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            
            #region Build/Mask Provider Request

            string requestBody = string.Empty;

            if (request.Method != HttpMethod.Get)
            {
                var requestMessage = request.Content.ReadAsByteArrayAsync().Result;
                requestBody = Encoding.UTF8.GetString(requestMessage);

                if (request.Content.Headers.ContentType.MediaType.Contains("xml"))
                {
                    var doc = new XmlDocument { XmlResolver = null };
                    doc.LoadXml(requestBody);

                    requestBody = StripNonAscii(HideFrontandBackImageMessage(doc));
                }
                else
                {
                    requestBody = StripNonAscii(requestBody, 1000);
                }
                // Mask requestBody when logging.
                if (GetMaskingEnabled())
                {
                    //requestBody = MaskEngine.MaskMessage(requestBody);
                }
            }


            var requestInfoData = new
            {
                requestStartDateTime = DateTime.Now,
                requestUri = request.RequestUri,
                requestBody,
                requestInfo = $@"{{""InternalServiceRequestUri"": ""{request.RequestUri}""}}"
            };

            var messageId = Guid.NewGuid().ToString();
            string correlationId = GetHeaderStringValue(request, "CorrelationId");

            var parsedValue = GetHeaderStringValue(request, "ProviderKey");
            int providerKey = string.IsNullOrEmpty(parsedValue) ? 0 : Convert.ToInt32(parsedValue);
            var transactionId = GetHeaderStringValue(request, "TransactionId") ?? string.Empty;
            if (string.IsNullOrEmpty(transactionId))
                transactionId = "None";

            _backgroundTaskQueue.QueueBackgroundWorkItem(
                    token => Task.Run(
                        () => _logRequestResponseRepository.LogInformation(@"request: {requestInfoData.requestBody ?? string.Empty}"),
                        token));

            #endregion

            #region External Service Call

            HttpResponseMessage response;
            try
            {
                response = base.SendAsync(request, cancellationToken).Result;
            }
            catch (Exception ex)
            {
                _backgroundTaskQueue.QueueBackgroundWorkItem(token =>
                    Task.Run(() => _logRequestResponseRepository.LogError(ex, @"{messageId}, {correlationId}, {transactionId}, {requestInfoData.requestUri.ToString()}, {DateTime.Now}"), token));
                throw;
            }

            #endregion

            #region Build/Mask Provider Response

            var responseMessage = string.Empty;
            var errorMessage = string.Empty;
            var responseCode = ((int)response.StatusCode).ToString();

            if (response.StatusCode == HttpStatusCode.InternalServerError ||
                response.StatusCode == HttpStatusCode.MethodNotAllowed ||
                response.StatusCode == HttpStatusCode.BadRequest)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.InternalServerError:
                        errorMessage = HttpErrorReason500;
                        break;
                    case HttpStatusCode.MethodNotAllowed:
                        errorMessage = HttpErrorReason405;
                        break;
                    default:
                        errorMessage = HttpErrorReason400;
                        break;
                }

                try
                {
                    if (response.Content != null)
                    {
                        // try to read the actual error response from internal service call
                        // but leave the default response in case unable to access the 
                        // underlying content stream
                        errorMessage = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        // no content returned, so only take response header and type information
                        responseMessage = response.ToString();
                    }
                }
                catch (Exception)
                {
                    // fallback to getting just type names for content instead of actual content
                    responseMessage = response.ToString();

                }
            }
            else if (response.Content != null)
            {
                responseMessage = Encoding.UTF8.GetString(response.Content.ReadAsByteArrayAsync().Result);
            }
            else
            {
                responseMessage = response.ToString();
            }

            #endregion
            //var responseDateTime = DateTime.Now;

            #region Log request and responses

            if (providerKey > 0)
            {

                try
                {
                    if (response.Content?.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType.Contains("xml"))
                    {
                        var doc = new XmlDocument { XmlResolver = null };
                        doc.LoadXml(responseMessage);

                        responseMessage = StripNonAscii(HideFrontandBackImageMessage(doc));
                    }
                    else
                    {
                        responseMessage = StripNonAscii(responseMessage, 1000);
                    }

                    // Mask responseMessage if masking is enabled.
                    if (GetMaskingEnabled())
                    {
                        //responseMessage = MaskEngine.MaskMessage(responseMessage);
                    }


                    _backgroundTaskQueue.QueueBackgroundWorkItem(token => Task.Run(() => 
                            _logRequestResponseRepository.LogInformation(@"{messageId}, {correlationId}, {transactionId}, {requestInfoData.requestUri.ToString()}, {DateTime.Now}, {responseMessage ?? string.Empty}"), token)
                    );


                }
                catch (InvalidOperationException)
                {
                    // trap exception which occurs during unit tests which
                    // do not have context.

                    try
                    {
                        //_backgroundTaskQueue.QueueBackgroundWorkItem(token =>
                        //    Task.Run((() => _logRequestResponseRepository.InsertProviderRequestResponse((short)providerKey,
                        //        messageId,
                        //        correlationId, transactionId, requestInfoData.requestUri.ToString(),
                        //        requestInfoData.requestStartDateTime, requestInfoData.requestBody ?? string.Empty, DateTime.Now, responseMessage ?? string.Empty)), token)
                        //);

                        _backgroundTaskQueue.QueueBackgroundWorkItem(token => Task.Run(() =>
                                _logRequestResponseRepository.LogInformation(@"{messageId}, {correlationId}, {transactionId}, {requestInfoData.requestUri.ToString()}, {DateTime.Now}, {responseMessage ?? string.Empty}"), token)
                        );


                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            #endregion


            return response;
        }

        private string HideFrontandBackImageMessage(XmlDocument doc)
        {
            var frontImageNodeList = doc.GetElementsByTagName("FrontImage");
            if (frontImageNodeList.Count > 0) frontImageNodeList[0].InnerText = "**Message too long for Logging**";
            var rearImageNodeList = doc.GetElementsByTagName("RearImage");
            if (rearImageNodeList.Count > 0) rearImageNodeList[0].InnerText = "**Message too long for Logging**";
            var BackImageNodeList = doc.GetElementsByTagName("BackImage");
            if (BackImageNodeList.Count > 0) BackImageNodeList[0].InnerText = "**Message too long for Logging**";

            return ConvertXmlToString(doc);
        }

        private string ConvertXmlToString(XmlDocument xmlDoc)
        {
            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter tx = new XmlTextWriter(sw))
                {
                    xmlDoc.WriteTo(tx);
                    string strXmlText = sw.ToString();
                    return strXmlText;
                }
            }
        }

        private static string GetHeaderStringValue(HttpRequestMessage request, string headerKey)
        {
            var headerValue = string.Empty;

            if (request.Headers.Contains(headerKey))
            {
                headerValue = request.Headers.GetValues(headerKey).First();
                request.Headers.Remove(headerKey);
            }

            return headerValue;
        }

        private static bool GetMaskingEnabled()
        {
            bool maskingEnabled;

            if (!bool.TryParse(ConfigurationManager.AppSettings[MaskingRestrictedDataEnabled], out maskingEnabled))
            {
                maskingEnabled = true; // default to true
            }

            return maskingEnabled;
        }

        private static string StripNonAscii(string input, int maxLength = -1)
        {
            string result;

            //result = Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8,
            //    Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback(string.Empty),
            //        new DecoderExceptionFallback()), Encoding.UTF8.GetBytes(input)));


            //var result = Regex.Replace(inputString, @"[^\u0000-\u007F]+", string.Empty); // ASCII
            //var result = Regex.Replace(inputString, @"[^\u0000-\u00FF]+", String.Empty); // extended ASCII

            // if string doesn't contain binary data
            if (string.IsNullOrWhiteSpace(input) || Encoding.UTF8.GetByteCount(input) == input.Length)
            {
                result = input;
            }
            else
            {
                const string fallbackStr = "";

                var enc = Encoding.GetEncoding(Encoding.ASCII.CodePage, new EncoderReplacementFallback(fallbackStr),
                    new DecoderReplacementFallback(fallbackStr));

                var valueString = enc.GetString(enc.GetBytes(input));

                var sb = new StringBuilder();

                foreach (var c in valueString)
                {
                    if (IsPrintableChar(c))
                    {
                        sb.Append(c);

                        if (maxLength >= 0 && sb.Length >= maxLength)
                        {
                            break;
                        }
                    }
                }

                result = sb.ToString();
            }

            return result;
        }

        private static bool IsPrintableChar(char c)
        {
            return !char.IsControl(c);
        }
    }
}
