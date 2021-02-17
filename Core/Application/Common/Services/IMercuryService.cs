using System.Threading.Tasks;
using Core.Application.Usecases.MercuryIntegration.ViewModels;

namespace Core.Application.Common.Services
{
    public interface IMercuryService
    {
        EnumElementListViewModel GetVsdTypeListViewModel();
        
        EnumElementListViewModel GetVsdStatusListViewModel();
        
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

        Task<object> ProcessIncomingConsignment(
            string uuid,
            string enterpriseId,
            string localTransactionId,
            string initiatorLogin
        );
    }
}
