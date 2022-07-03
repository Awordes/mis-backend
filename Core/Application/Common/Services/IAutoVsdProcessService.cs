using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Common.Services
{
    public interface IAutoVsdProcessService
    {
        Task StartProcessing (CancellationToken cancellationToken);
    }
}