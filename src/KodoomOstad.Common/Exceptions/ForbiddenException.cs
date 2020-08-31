using System.Net;

namespace KodoomOstad.Common.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Forbidden Exception")
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }
}
