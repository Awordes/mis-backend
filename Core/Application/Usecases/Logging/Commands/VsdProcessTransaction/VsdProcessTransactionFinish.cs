using System;
using MediatR;

namespace Core.Application.Usecases.Logging.Commands.VsdProcessTransaction
{
    public class VsdProcessTransactionFinish: IRequest
    {
        public Guid VsdProcessTransactionId { get; set; }

        public string Error { get; set; }
    }
}