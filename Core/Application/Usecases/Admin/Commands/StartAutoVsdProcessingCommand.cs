using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using MediatR;

namespace Core.Application.Usecases.Admin.Commands
{
    public class StartAutoVsdProcessingCommand: IRequest
    {
        private class Handler: IRequestHandler<StartAutoVsdProcessingCommand>
        {
            private readonly IAutoVsdProcessingStartService _autoVsdProcessingStartService;

            public Handler(IAutoVsdProcessingStartService autoVsdProcessingStartService)
            {
                _autoVsdProcessingStartService = autoVsdProcessingStartService;
            }

            public async Task<Unit> Handle(StartAutoVsdProcessingCommand request, CancellationToken cancellationToken)
            {
                await _autoVsdProcessingStartService.StartAutoVsdProcessing(cancellationToken);
                return Unit.Value;
            }
        }
    }
}