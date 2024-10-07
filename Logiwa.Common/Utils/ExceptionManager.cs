using Logiwa.Common.Base;
using System.Net;

namespace Logiwa.Common.Utils
{
    public class ExceptionManager : IExceptionManager
    {
        public IErrorResponse ConstructExceptionModel(Exception thrownException)
        {
            int code = GetHttpStatusCode(thrownException) ?? (int)HttpStatusCode.InternalServerError;

            bool isServerException = code / 100 == (int)HttpStatusCode.InternalServerError / 100;
            string errorMessage = isServerException ? "Unknown Error" : thrownException.Message;
            string exceptionContent = isServerException ? thrownException.ToString() : string.Empty;
            string innerException = thrownException?.InnerException?.Message ?? string.Empty;

            var errorResponse = new ErrorResponse
            {
                Code = code,
                Message = errorMessage,
                InnerException = innerException,
                ExceptionContent = exceptionContent
            };

            return errorResponse;
        }

        private int? GetHttpStatusCode<TException>(TException exception)
          where TException : Exception
        {
            //if (_exceptionCodePair.TryGetValue(exception.GetType(), out var code))
            //{
            //    return code;
            //}

            return null;
        }
    }
}
