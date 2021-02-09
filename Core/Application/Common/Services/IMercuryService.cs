using System.Threading.Tasks;

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

        Task<object> GetVetDocumentByUuid(
            string uuid,
            string enterpriseId,
            string localTransactionId,
            string initiatorLogin
        );
    }
}
