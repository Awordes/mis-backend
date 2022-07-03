using System;

namespace Infrastructure.Exceptions
{
    public class MercuryServiceException: Exception
    {
        public string VsdId { get; }
        
        public Guid EnterpriseId { get; }

        public MercuryServiceException(string message, string vsdId, Guid enterpriseId) : base(message)
        {
            VsdId = vsdId;

            EnterpriseId = enterpriseId;
        }
    }
}