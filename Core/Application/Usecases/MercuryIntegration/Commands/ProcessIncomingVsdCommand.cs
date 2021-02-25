using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using Core.Application.Usecases.Logging.Operations;
using Core.Application.Usecases.Logging.VsdProcessTransaction;
using Core.Domain.Auth;
using Core.Domain.Operations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.MercuryIntegration.Commands
{
    public class ProcessIncomingVsdCommand: IRequest
    {
        /// <summary>
        /// Идентификатор ВСД
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Идентификатор предприятия
        /// </summary>
        public Guid EnterpriseId { get; set; }
        
        private class Handler: IRequestHandler<ProcessIncomingVsdCommand>
        {
            private readonly IMercuryService _mercuryService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<User> _userManager;
            private readonly IMisDbContext _context;
            private readonly IMediator _mediator;

            private Guid _operationId;
            private Guid _vsdProcessTransactionId;

            public Handler(
                IMercuryService mercuryService,
                IHttpContextAccessor httpContextAccessor,
                UserManager<User> userManager,
                IMisDbContext context,
                IMediator mediator)
            {
                _mercuryService = mercuryService;
                _httpContextAccessor = httpContextAccessor;
                _userManager = userManager;
                _context = context;
                _mediator = mediator;
            }
            
            public async Task<Unit> Handle(ProcessIncomingVsdCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;

                    var user = await _userManager.FindByNameAsync(userName)
                        ?? throw new Exception($@"Пользователь с именем {userName} не найден.");

                    var enterprise = await _context.Enterprises.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id == request.EnterpriseId, cancellationToken)
                        ?? throw new Exception($@"Предприятие с идентификатором {request.EnterpriseId} не найден.");

                    _operationId = await _mediator.Send(new OperationStart
                    {
                        UserId = user.Id,
                        Type = OperationType.VsdProcess
                    }, cancellationToken);

                    _vsdProcessTransactionId = await _mediator.Send(new VsdProcessTransactionStart
                    {
                        OperationId = _operationId,
                        VsdId = request.Uuid
                    }, cancellationToken);
                    
                    await _mercuryService.ProcessIncomingConsignment(
                        "a10003",
                        user,
                        enterprise,
                        request.Uuid
                    );

                    await _mediator.Send(new VsdProcessTransactionFinish
                    {
                        VsdProcessTransactionId = _vsdProcessTransactionId
                    }, cancellationToken);
                    
                    await _mediator.Send(new OperationFinish
                    {
                        OperationId = _operationId
                    }, cancellationToken);
                    
                    return Unit.Value;
                }
                catch (Exception e)
                {
                    await _mediator.Send(new VsdProcessTransactionFinish
                    {
                        VsdProcessTransactionId = _vsdProcessTransactionId,
                        Error = e.Message
                    }, cancellationToken);

                    await _mediator.Send(new OperationFinish
                    {
                        OperationId = _operationId
                    }, cancellationToken);
                    
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}