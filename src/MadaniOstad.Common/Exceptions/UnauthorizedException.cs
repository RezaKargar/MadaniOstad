using System.Net;

namespace MadaniOstad.Common.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Unauthorized Exception")
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }
}
