using Logiwa.Common.Base;

namespace Logiwa.Common.Utils
{
    public interface IExceptionManager
    {
        IErrorResponse ConstructExceptionModel(Exception exception);
    }
}
