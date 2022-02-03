using System;

namespace Infrastructure.Exceptions
{
    public class MercuryEnterpriseNotFoundException : Exception
    {
        public MercuryEnterpriseNotFoundException(string message) : base (message)
        { }
    }
}