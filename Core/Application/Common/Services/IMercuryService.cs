using System.Threading.Tasks;
using Core.Application.Usecases.MercuryIntegration.ViewModels;
using Core.Domain.Auth;

namespace Core.Application.Common.Services
{
    public interface IMercuryService
    {
        EnumElementListViewModel GetVsdTypeListViewModel();
        
        EnumElementListViewModel GetVsdStatusListViewModel();
        
        Task<VsdListViewModel> GetVetDocumentList(
            string localTransactionId,
            User user,
            int count,
            int offset,
            int vetDocumentType,
            int vetDocumentStatus
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
