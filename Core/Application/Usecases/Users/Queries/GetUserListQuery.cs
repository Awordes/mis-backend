using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Extensions;
using Core.Application.Common.Pagination;
using Core.Application.Usecases.Users.ViewModels;
using Core.Domain.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Users.Queries
{
    public class GetUserListQuery: IRequest<PagedResult<UserViewModel>>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
        
        private class Handler: IRequestHandler<GetUserListQuery, PagedResult<UserViewModel>>
        {
            private readonly IMisDbContext _context;
            private readonly IMapper _mapper;

            public Handler(IMisDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PagedResult<UserViewModel>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    return await _context.Users.AsNoTracking().Include(x => x.Enterprises).OrderBy(x => x.UserName)
                        .GetPagedAsync<User, UserViewModel>(request.Page, request.PageSize, _mapper, cancellationToken);
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