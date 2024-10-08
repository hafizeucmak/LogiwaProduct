using System.ComponentModel.DataAnnotations;

namespace Logiwa.Common.Exceptions
{
    public class ResourceNotFoundException : ValidationException
    {
        public ResourceNotFoundException(string message)
            : base(message)
        {
        }
    }
}
