using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DAM.Application.Users.Exceptions
{
    public class UserException : Exception
    {
        public UserException()
        {
        }

        public UserException(string message) : base(message)
        {
        }

        public UserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
