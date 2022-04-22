using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Common.Services
{
    public interface IAutoVsdProcessingStartService
    {
        Task StartAutoVsdProcessing(CancellationToken cancellationToken);
    }
}