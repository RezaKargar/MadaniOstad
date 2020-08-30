using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace KodoomOstad.Common.Exceptions
{
    public class InternalException : AppException
    {
        public InternalException(string message = "Internal Server Error")
            : base(message, HttpStatusCode.InternalServerError)
        {
        }
    }
}
