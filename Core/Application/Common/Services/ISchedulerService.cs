using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Common.Services
{
    public interface ISchedulerService
    {
        Task AutoProcessVsd(CancellationToken cancellationToken = default);
    }
}