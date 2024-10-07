namespace Logiwa.WebAPI.Middlewares
{
    internal class ErrorResponse
    {
        public object StatusCode { get; set; }
        public object Message { get; set; }
        public string? InnerException { get; set; }
        public object ExceptionContent { get; set; }
    }
}