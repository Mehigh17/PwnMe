using System;

namespace PwnMe.Exceptions
{
    public class PwnMeApiErrorException : Exception
    {
        public PwnMeApiErrorException()
        {

        }

        public PwnMeApiErrorException(string message)
            : base(message)
        {

        }

        public PwnMeApiErrorException(string message, Exception inner)
            : base(message, inner)
        {

        }

    }
}