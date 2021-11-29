using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Operations;

namespace Core.Application.Common.Services
{
    public interface ILogService
    {
        public Task<Guid> StartOperation(Guid userId, OperationType operationType, CancellationToken cancellationToken = default);

        public Task FinishOperation(Guid operationId, CancellationToken cancellationToken = default);

        public Task<Guid> StartVsdProcessTransaction(Guid operationId, string vsdId,
            CancellationToken cancellationToken = default);

        public Task FinishVsdProcessTransaction(Guid vsdProcessTransactionId, string error,
            CancellationToken cancellationToken = default);
    }
}