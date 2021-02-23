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

namespace Core.Application.Usecases.MercuryIntegration.Queries
{
    public class GetVetDocumentByUuidQuery: IRequest<object>
    {
        /// <summary>
        /// Идентификатор ВСД
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Идентификатор предприятия
        /// </summary>
        public Guid EnterpriseId { get; set; }
        
        private class Handler: IRequestHandler<GetVetDocumentByUuidQuery, object>
        {
            private readonly IMercuryService _mercuryService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMisDbContext _context;
            private readonly UserManager<User> _userManager;

            public Handler(
                IMercuryService mercuryService,
                IHttpContextAccessor httpContextAccessor,
                IMisDbContext context,
                UserManager<User> userManager)
            {
                _mercuryService = mercuryService;
                _httpContextAccessor = httpContextAccessor;
                _context = context;
                _userManager = userManager;
            }
            
            public async Task<object> Handle(GetVetDocumentByUuidQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;

                    var user = await _userManager.FindByNameAsync(userName)
                        ?? throw new Exception($@"Пользователь с именем {userName} не найден.");

                    var enterprise = await _context.Enterprises.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id == request.EnterpriseId, cancellationToken)
                        ?? throw new Exception($@"Предприятие с идентификатором {request.EnterpriseId} не найден.");

                    return await _mercuryService.GetVetDocumentByUuid(
                        "a10003",
                        user,
                        enterprise,
                        request.Uuid
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