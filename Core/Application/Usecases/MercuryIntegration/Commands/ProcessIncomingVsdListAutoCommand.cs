using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using Core.Application.Usecases.MercuryIntegration.Models;
using Core.Domain.Auth;
using Core.Domain.Operations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Usecases.MercuryIntegration.Commands
{
    public class ProcessIncomingVsdListAutoCommand: IRequest
    {
        public ICollection<VsdForProcessModel> Vsds { get; init; }

        public User User { get; set; }

        public Enterprise Enterprise { get; set; }
        
        private class Handler: IRequestHandler<ProcessIncomingVsdListAutoCommand>
        {
            private readonly IMercuryService _mercuryService;
            private readonly ILogService _logService;
            private readonly ILogger<Handler> _logger;

            private Guid _operationId;

            public Handler(ILogger<Handler> logger, ILogService logService, IMercuryService mercuryService)
            {
                _logger = logger;
                _logService = logService;
                _mercuryService = mercuryService;
            }

            public async Task<Unit> Handle(ProcessIncomingVsdListAutoCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    try
                    {
                        _operationId = await _logService.StartOperation(request.User.Id, OperationType.VsdProcess, cancellationToken);
                        
                        var tasks = new List<Task>();

                        foreach (var vsd in request.Vsds)
                        {
                            tasks.Add(_mercuryService.ProcessIncomingConsignment(
                                _operationId.ToString(), request.User, request.Enterprise, vsd.VsdId, vsd.ProcessDate, _operationId));

                            if (tasks.Count != 4) continue;
                            
                            Task.WaitAll(tasks.ToArray(), cancellationToken);
                            
                            tasks = new List<Task>();
                        }
                        
                        Task.WaitAll(tasks.ToArray(), cancellationToken);
                    }
                    finally
                    {
                        await _logService.FinishOperation(_operationId, cancellationToken);
                    }
                    
                    return Unit.Value;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    throw;
                }
            }
        }
    }
}