using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.MercuryIntegration.Commands
{
    public class FinishVetDocumentCommand: IRequest
    {
        /// <summary>
        /// Идентификатор ВСД
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Идентификатор предприятия
        /// </summary>
        public Guid EnterpriseId { get; set; }
        
        private class Handler: IRequestHandler<FinishVetDocumentCommand>
        {
            private readonly IMercuryService _mercuryService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<User> _userManager;
            private readonly IMisDbContext _context;

            public Handler(
                IMercuryService mercuryService,
                IHttpContextAccessor httpContextAccessor,
                UserManager<User> userManager,
                IMisDbContext context)
            {
                _mercuryService = mercuryService;
                _httpContextAccessor = httpContextAccessor;
                _userManager = userManager;
                _context = context;
            }
            
            public async Task<Unit> Handle(FinishVetDocumentCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;

                    var user = await _userManager.FindByNameAsync(userName)
                        ?? throw new Exception($@"Пользователь с именем {userName} не найден.");

                    var enterprise = await _context.Enterprises.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id == request.EnterpriseId, cancellationToken)
                        ?? throw new Exception($@"Предприятие с идентификатором {request.EnterpriseId} не найден.");
                    
                    await _mercuryService.ProcessIncomingConsignment(
                        "a10003",
                        user,
                        enterprise,
                        request.Uuid
                    );
                    
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