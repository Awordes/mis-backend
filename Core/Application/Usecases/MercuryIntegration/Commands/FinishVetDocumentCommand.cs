using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace Core.Application.Usecases.MercuryIntegration.Commands
{
    public class FinishVetDocumentCommand: IRequest<object>
    {
        public string Uuid { get; set; }
        
        private class Handler: IRequestHandler<FinishVetDocumentCommand, object>
        {
            private readonly MercuryOptions _mercuryOptions;
            private readonly IMercuryService _mercuryService;

            public Handler(
                IOptionsMonitor<MercuryOptions> mercuryOptions,
                IMercuryService mercuryService)
            {
                _mercuryOptions = mercuryOptions.CurrentValue;
                _mercuryService = mercuryService;
            }
            
            public async Task<object> Handle(FinishVetDocumentCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    return await _mercuryService.ProcessIncomingConsignment(
                        request.Uuid,
                        _mercuryOptions.EnterpriseId,
                        _mercuryOptions.LocalTransactionId,
                        _mercuryOptions.InitiatorLogin
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}