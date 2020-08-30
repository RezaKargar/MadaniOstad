using System.Net;

namespace KodoomOstad.Common.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message = "Bad Request")
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}
