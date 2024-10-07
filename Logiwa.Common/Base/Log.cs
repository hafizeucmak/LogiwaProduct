namespace Logiwa.Common.Base
{
    public class Log
    {
        public Log(string url,
                   string requestBody,
                   string queryString,
                   string operationName,
                   string method)
        {
            Url = url;
            RequestBody = requestBody;
            QueryString = queryString;
            OperationName = operationName;
            Method = method;
        }

        public string OperationName { get; set; }

        public string RequestBody { get; set; }

        public string QueryString { get; set; }

        public string Url { get; set; }

        public string? ResponseBody { get; set; }

        public string Method { get; set; }

        public bool Success { get; set; }

        public int ResultCode { get; set; }
    }

    public class LogExceptionDetails
    {
        public string? StackTrace { get; set; }

        public string? InnerException { get; set; }

        public string? ExceptionMessage { get; set; }

        public int? ResultCode { get; set; }
    }
}
