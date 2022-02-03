using System;

namespace Infrastructure.Exceptions
{
    public class MercuryServiceException : Exception
    {
        public MercuryServiceException(string message) :base(message)
        { }
    }
}