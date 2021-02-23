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
            Enterprise enterprise,
            int count,
            int offset,
            int vetDocumentType,
            int vetDocumentStatus
        );

        Task<object> GetVetDocumentByUuid(
            string localTransactionId,
            User user,
            Enterprise enterprise,
            string uuid
        );

        Task ProcessIncomingConsignment(
            string localTransactionId,
            User user,
            Enterprise enterprise,
            string uuid
        );
    }
}
