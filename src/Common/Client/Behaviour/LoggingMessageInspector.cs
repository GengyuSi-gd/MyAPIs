using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MMS.Service.CheckDeposit.Repository.Common.Behaviour
{
    /*public sealed class LoggingMessageInspector : IClientMessageInspector
    {
        private readonly ServiceEndpoint _targetServiceEndPoint;
        private readonly IRequestResponseLogRepository _requestResponseLogRepository;

        public LoggingMessageInspector(ServiceEndpoint serviceEndpoint)
        {
            _targetServiceEndPoint = serviceEndpoint;
           _requestResponseLogRepository=new DbRequestResponseLogRepository();
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(reply.ToString());
            
            string responseMessage = StripNonAscii(HideFrontandBackImageMessage(doc));
            responseMessage = MaskEngine.MaskMessage(responseMessage);
            
            var messageId = HttpContext.Current.Request.Headers[CustomHttpHeaderKey.MessageId];
            if (messageId!=null)
            {
                HostingEnvironment.QueueBackgroundWorkItem(
                    obj =>
                _requestResponseLogRepository.LogProviderResponse(
                    messageId, _targetServiceEndPoint.ListenUri.ToString(), DateTime.Now,
                    responseMessage ?? string.Empty));
            }
     
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(request.ToString());

            

            string requestMessage = StripNonAscii(HideFrontandBackImageMessage(doc));
            requestMessage = MaskEngine.MaskMessage(requestMessage);
            
            var loggingValues = SetLoggingValues();
            if (loggingValues.Count>0)
            {
                var parsedValue = loggingValues[CustomHttpHeaderKey.ProviderKey];
                int providerKey = string.IsNullOrEmpty(parsedValue) ? 0 : Convert.ToInt32(parsedValue);
                if (providerKey > 0)
                {
                    HostingEnvironment.QueueBackgroundWorkItem(
                        obj =>
                            _requestResponseLogRepository.LogProviderRequest((short)providerKey, loggingValues[CustomHttpHeaderKey.MessageId], loggingValues[CustomHttpHeaderKey.CorrelationId], loggingValues[CustomHttpHeaderKey.TransactionId], _targetServiceEndPoint.ListenUri.ToString(), DateTime.Now, requestMessage));
                }
            }
           

            return -1;
        }


        private Dictionary<string, string> SetLoggingValues()
        {
            var owContext = WebOperationContext.Current;
            var loggingValues=new Dictionary<string,string>();

            if (owContext!=null)
            {
                var messageid = Guid.NewGuid().ToString();
                HttpContext.Current.Request.Headers.Remove(CustomHttpHeaderKey.MessageId);
                HttpContext.Current.Request.Headers.Add(CustomHttpHeaderKey.MessageId, messageid);
                loggingValues.Add(CustomHttpHeaderKey.MessageId, messageid);

                var correlationId = owContext.OutgoingRequest.Headers[CustomHttpHeaderKey.CorrelationId];
                loggingValues.Add(CustomHttpHeaderKey.CorrelationId,correlationId);
                // ReSharper disable once PossibleNullReferenceException
                
                    var transactionId = owContext.OutgoingRequest.Headers[CustomHttpHeaderKey.TransactionId];
                if (string.IsNullOrEmpty(transactionId))
                    transactionId = "None";
                loggingValues.Add(CustomHttpHeaderKey.TransactionId,transactionId);

                var providerKey = owContext.OutgoingRequest.Headers[CustomHttpHeaderKey.ProviderKey];
                loggingValues.Add(CustomHttpHeaderKey.ProviderKey,providerKey);
            }
            

            return loggingValues;
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

    }*/
}
