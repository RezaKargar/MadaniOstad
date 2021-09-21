using System.Net;

namespace MadaniOstad.Common.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Forbidden Exception")
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }
}
