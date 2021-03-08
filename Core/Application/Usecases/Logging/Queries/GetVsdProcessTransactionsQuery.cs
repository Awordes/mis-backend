using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Extensions;
using Core.Application.Common.Pagination;
using Core.Application.Usecases.Logging.ViewModels;
using Core.Application.Usecases.Users.Queries;
using Core.Domain.Operations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Logging.Queries
{
    public class GetVsdProcessTransactionsQuery: IRequest<PagedResult<VsdProcessTransactionViewModel>>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
        
        private class Handler: IRequestHandler<GetVsdProcessTransactionsQuery, PagedResult<VsdProcessTransactionViewModel>>
        {
            private readonly IMisDbContext _context;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;
            
            public Handler(IMisDbContext context, IMapper mapper, IMediator mediator)
            {
                _context = context;
                _mapper = mapper;
                _mediator = mediator;
            }
            
            public async Task<PagedResult<VsdProcessTransactionViewModel>> Handle(GetVsdProcessTransactionsQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken); 
                    
                    return await _context.VsdProcessTransactions.AsNoTracking()
                        .Include(x => x.Operation)
                            .ThenInclude(x => x.User)
                        .Where(x => x.Operation.User.Id == user.Id)
                        .OrderByDescending(x => x.StartTime)
                        .GetPagedAsync<VsdProcessTransaction, VsdProcessTransactionViewModel>
                            (request.Page, request.PageSize, _mapper, cancellationToken);
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