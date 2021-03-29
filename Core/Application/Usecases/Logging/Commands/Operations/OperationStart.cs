using System;
using Core.Domain.Operations;
using MediatR;

namespace Core.Application.Usecases.Logging.Commands.Operations
{
    public class OperationStart: IRequest<Guid>
    {
        public Guid UserId { get; set; }

        public OperationType Type { get; set; }
    }
}