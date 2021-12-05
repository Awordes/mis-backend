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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.MercuryIntegration.Commands
{
    public class ProcessIncomingVsdListCommand: IRequest
    {
        /// <summary>
        /// Идентификатор предприятия
        /// </summary>
        public Guid EnterpriseId { get; init; }

        /// <summary>
        /// Идентификаторы ВСД
        /// </summary>
        public ICollection<VsdForProcessModel> Vsds { get; init; }
        
        private class Handler: IRequestHandler<ProcessIncomingVsdListCommand>
        {
            private readonly IMercuryService _mercuryService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<User> _userManager;
            private readonly IMisDbContext _context;
            private readonly ILogService _logService;

            private Guid _operationId;

            public Handler(IMercuryService mercuryService, IHttpContextAccessor httpContextAccessor,
                UserManager<User> userManager, IMisDbContext context, ILogService logService)
            {
                _mercuryService = mercuryService;
                _httpContextAccessor = httpContextAccessor;
                _userManager = userManager;
                _context = context;
                _logService = logService;
            }
            
            public async Task<Unit> Handle(ProcessIncomingVsdListCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;

                    var user = await _userManager.FindByNameAsync(userName)
                        ?? throw new Exception($@"Пользователь с именем {userName} не найден.");

                    var enterprise = await _context.Enterprises.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id == request.EnterpriseId, cancellationToken)
                        ?? throw new Exception($@"Предприятие с идентификатором {request.EnterpriseId} не найдено.");

                    try
                    {
                        _operationId = await _logService.StartOperation(user.Id, OperationType.VsdProcess, cancellationToken);
                        
                        var tasks = new List<Task>();

                        foreach (var vsd in request.Vsds)
                        {
                            tasks.Add(_mercuryService.ProcessIncomingConsignment(
                                _operationId.ToString(), user, enterprise, vsd.VsdId, vsd.ProcessDate, _operationId));

                            if (tasks.Count != 4) continue;
                            
                            Task.WaitAll(tasks.ToArray(), cancellationToken);
                            
                            tasks = new List<Task>();
                        }
                        
                        Task.WaitAll(tasks.ToArray(), cancellationToken);
                    }
                    finally
                    {
                        await _logService.FinishOperation(_operationId, cancellationToken);
                        
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    
                    return Unit.Value;
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