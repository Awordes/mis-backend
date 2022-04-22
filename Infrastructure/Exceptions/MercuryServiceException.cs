using System;

namespace Infrastructure.Exceptions
{
    public class MercuryServiceException: Exception
    {
        public string VsdId { get; }

        public MercuryServiceException(string message, string vsdId) : base(message)
        {
            VsdId = vsdId;
        }
    }
}