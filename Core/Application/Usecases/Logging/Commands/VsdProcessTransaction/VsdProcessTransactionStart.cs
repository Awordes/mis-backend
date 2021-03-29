using System;
using MediatR;

namespace Core.Application.Usecases.Logging.Commands.VsdProcessTransaction
{
    public class VsdProcessTransactionStart: IRequest<Guid>
    {
        public Guid OperationId { get; set; }

        public string VsdId { get; set; }
    }
}