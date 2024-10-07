using Logiwa.Common.Base;
using Logiwa.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Logiwa.WebAPI.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext,
                                    IExceptionManager exceptionManager,
                                    ILogger<ExceptionHandlerMiddleware> logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception thrownException)
            {
                var errorResponse = exceptionManager.ConstructExceptionModel(thrownException);

                string exceptionMessageBody = JsonConvert.SerializeObject(errorResponse, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                });

                httpContext.Response.StatusCode = errorResponse.Code;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(exceptionMessageBody);

                var logException = new LogExceptionDetails();
                logException.StackTrace = thrownException.StackTrace;
                logException.InnerException = thrownException.InnerException?.Message;
                logException.ExceptionMessage = thrownException.Message;
                logException.ResultCode = httpContext.Response.StatusCode;

                logger.LogError("Exception Log Details: {@Log}", logException);
            }
        }
    }
}
