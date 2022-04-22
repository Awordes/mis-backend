using System;

namespace Infrastructure.Exceptions
{
    public class MercuryOffsetException: Exception
    {
        public MercuryOffsetException(string message): base(message)
        { }
    }
}