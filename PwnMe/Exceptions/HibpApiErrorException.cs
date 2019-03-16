using System;

namespace PwnMe.Exceptions
{
    public class HibpApiErrorException : Exception
    {
        public HibpApiErrorException()
        {

        }

        public HibpApiErrorException(string message)
            : base(message)
        {

        }

        public HibpApiErrorException(string message, Exception inner)
            : base(message, inner)
        {

        }

    }
}