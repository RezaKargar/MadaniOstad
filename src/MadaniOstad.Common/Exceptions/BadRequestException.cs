using System.Net;

namespace MadaniOstad.Common.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message = "Bad Request")
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}
