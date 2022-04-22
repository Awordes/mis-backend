using System;

namespace Infrastructure.Exceptions
{
    public class MercuryRequestRejectedException : Exception
    {
        public MercuryRequestRejectedException(string message) :base(message)
        { }
    }
}