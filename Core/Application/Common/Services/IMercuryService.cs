using System.Threading.Tasks;
using System;

namespace Core.Application.Common.Services
{
    public interface IMercuryService
    {
        Task<object> GetVetDocumentList(
            string localTransactionId,
            string initiatorLogin,
            int count,
            int offset,
            int vetDocumentType,
            int vetDocumentStatus,
            string enterpriseId
            );
    }
}
