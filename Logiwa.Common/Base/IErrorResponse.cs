namespace Logiwa.Common.Base
{
    public interface IErrorResponse
    {
        int Code { get; set; }

        string? Message { get; set; }

        string? InnerException { get; set; }

        string? ExceptionContent { get; set; }

        int StatusCode { get; set; }
    }
}
