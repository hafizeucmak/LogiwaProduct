using System.ComponentModel.DataAnnotations;

namespace Logiwa.Common.Exceptions
{
    public class AlreadyExistsException : ValidationException
    {
        public AlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}
