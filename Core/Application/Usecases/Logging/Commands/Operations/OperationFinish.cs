using System;
using MediatR;

namespace Core.Application.Usecases.Logging.Commands.Operations
{
    public class OperationFinish: IRequest
    {
        public Guid OperationId { get; set; }
    }
}