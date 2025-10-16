using System.Net;
using System.Reflection;
using System.Text;
using Business.Extensions;
using Business.Filters;
using Common.Log;
using Common.Request;
using Common.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace MyAPIs.Filters
{
    /// <summary>
    /// ActionFilter to log API request input and response outputs.
    /// Turn on and off from web.config entry "requestLogging".
    /// </summary>
    public class LogRequest : ActionFilterAttribute
    {
        private IConfiguration _configuration { get; }
        private readonly ILogger<LogRequest> _logManager;

        private const string TracingHeaderPrefix = "TracingHeaderPrefix";
        private const string TracingHeader = "RequestTraceId";
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public LogRequest(IBackgroundTaskQueue backgroundTaskQueue, IConfiguration configuration, ILogger<LogRequest> logManager)
        {
            _configuration = configuration;
            _logManager = logManager;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        // Request Logging
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request= FormatRequestMessage(context);
            if (!string.IsNullOrEmpty(request))
                _backgroundTaskQueue.QueueBackgroundWorkItem(token => Task.Run(() => DoRequestLogging(request), token));
            
            base.OnActionExecuting(context);
        }

        // Response Logging.
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var response = FormatResponseMessage(context);
            if (!string.IsNullOrEmpty(response))
                _backgroundTaskQueue.QueueBackgroundWorkItem(token => Task.Run(() => DoResponseLogging(response), token));
            base.OnActionExecuted(context);
        }

        private void DoRequestLogging(string  request)
        {
            try
            {
                
                // Log request.
                _logManager.LogInformation("Request: {0}", JsonConvert.SerializeObject(request));
            }
            catch (Exception e)
            {
                _logManager.LogError(e,"LogRequest failed");
            }
        }

        private string FormatRequestMessage(ActionExecutingContext context)
        {

            var request =string.Empty;

            try
            {
                var logRequest = _configuration["AppSettings:RequestLogging"]?.ToLower().Equals("true") ?? false;
                if (logRequest && context?.HttpContext?.Request != null)
                {
                    var sb = new StringBuilder();

                    #region Build Header Trace Value Logs
                    var headers = context.HttpContext.Request.Headers;
                    if (!(headers != null && headers.Keys.Contains(TracingHeader)))
                    {
                        // If there isn't already a TraceId in the header, add one.
                        context.HttpContext.Request.Headers.Add(TracingHeader,
                            $"tracingHeaderPrefix:{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}");
                    }
                    sb.Append($"{TracingHeader}={headers?[TracingHeader]} :: ");
                    #endregion

                    if (context.ActionArguments != null)
                    {
                        sb.Append($"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path} REQUEST ::{{ ");

                        #region Build Input Parameters Logs
                        foreach (var arg in context.ActionArguments)
                        {
                            if (arg.Value == null) continue;
                            var t = arg.Value.GetType();

                            // For each derivative of CheckDepositBaseRequest, use reflection to iterate through request properties.
                            sb.Append(t.IsSubclassOf(typeof(BaseRequest))
                                ? LogObjectProperties(t, arg.Value)
                                : $"{arg.Key}={arg.Value}, ");
                        }
                        #endregion
                    }
                    sb.Append("}");

                    // Log request.
                    request = sb.ToString();
                }

            }
            catch (Exception e)
            {
                _logManager.LogError(e, "FormatRequest failed");
            }

            return request;
        }

        private string FormatResponseMessage(ActionExecutedContext context)
        {
            var responseString = string.Empty;
            try
            {
                // Check if request logging is turned on from web.config
                var logRequest = _configuration["AppSettings:RequestLogging"]?.ToLower().Equals("true") ?? false;
                if (logRequest && context != null)
                {
                    StringBuilder sb = new StringBuilder();

                    #region Build Header Trace Value Logs
                    var headers = context.HttpContext?.Request?.Headers;
                    if (headers != null && headers.Keys.Contains(TracingHeader))
                    {
                        sb.Append($"{TracingHeader}={headers[TracingHeader]} :: ");
                    }
                    #endregion

                    if (context.HttpContext?.Response != null)
                    {
                        var objectResult = (ObjectResult)context.Result;

                        sb.Append(
                            $"{context.HttpContext?.Request?.Method} {context.HttpContext?.Request?.Path.Value} RESPONSE :: [{objectResult.StatusCode ?? 500}] {(objectResult.StatusCode.HasValue ? ((HttpStatusCode) System.Enum.ToObject(typeof(HttpStatusCode), objectResult.StatusCode.Value)).ToString() : HttpStatusCode.InternalServerError.ToString())} ::{{ ");

                        if (context.HttpContext.Response.Body != null)
                        {
                            var response = objectResult.Value;

                            #region Build Output Parameter Logs 
                            if (response != null)
                            {
                                var t = response.GetType();

                                // For each derivative of BaseResponse, use reflection to iterate through request properties.
                                sb.Append(t.IsSubclassOf(typeof(BaseResponse))
                                    ? LogObjectProperties(t, response)
                                    : $"{response} ");
                            }
                            #endregion
                        }
                    }
                    sb.Append("}");

                    responseString = sb.ToString();
                }
            }
            catch (Exception e)
            {
                _logManager.LogError(e, "FormatResponse failed");
            }

            return responseString;
        }

        private void DoResponseLogging(string response)
        {
            try
            {
                    // Log response.
                    _logManager.LogInformation("Request: {0}", JsonConvert.SerializeObject(response));
                
            }
            catch (Exception e)
            {
                _logManager.LogError(e, "LogResponse failed");
            }

        }

        private static string LogObjectProperties(Type t, object value)
        {
            var sb = new StringBuilder();
            var properties = t.GetProperties();
            foreach (var p in properties)
            {
                // Skip properties that are tagged with DoNotLog attribute to omit customer sensitive data.
                if (p.GetCustomAttributes<DoNotLogAttribute>().ToList().Count == 0)
                {
                    if (!p.IsList())
                    {
                        var o = p.GetValue(value, null);
                        if (o != null)
                        {
                            var objType = o.GetType();
                            if (objType.IsSubclassOf(typeof(BaseResponse)))
                            {
                                sb.Append($"{p.Name}=");
                                sb.Append("{");
                                sb.Append(LogObjectProperties(objType, o));
                                sb.Append("}, ");
                            }
                            else
                            {
                                sb.Append($"{p.Name}={o}, ");
                            }
                        }
                        else
                        {
                            sb.Append($"{p.Name}=null, ");
                        }
                    }
                    else
                    {
                        // Skip logging of lists to maintain log readability.
                        // sb.Append($"{p.Name}=(List), ");

                        // Log list objects.  Recursively call LogObjectProperties() for all nested lists, or log primitive/string value items.
                        sb.Append($"{p.Name}=[");
                        if (p.GetValue(value, null) is IEnumerable<object> objList && objList.Any())
                        {
                            Type objType = objList.First().GetType();
                            bool isPrimitiveOrString = objType.IsPrimitiveOrString();

                            foreach (var o in objList)
                            {
                                if (isPrimitiveOrString)
                                {
                                    sb.Append($"{o}, ");
                                }
                                else
                                {
                                    sb.Append("{");
                                    sb.Append(LogObjectProperties(objType, o));
                                    sb.Append("}, ");
                                }
                            }
                        }
                        sb.Append("], ");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
