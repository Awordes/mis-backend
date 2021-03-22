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
        /// <summary>
        /// Количество страниц
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Количество элементов на странице
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }

        /// <summary>
        /// Дата окончания подписки с
        /// </summary>
        public DateTime? ExpirationDateStart { get; set; }

        /// <summary>
        /// Дата окончания подписки по
        /// </summary>
        public DateTime? ExpirationDateEnd { get; set; }
        
        private class Handler: IRequestHandler<GetUserListQuery, PagedResult<UserViewModel>>
        {
            private readonly IMisDbContext _context;
            private readonly IMapper _mapper;

            public Handler(IMisDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PagedResult<UserViewModel>> Handle
                (GetUserListQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    IQueryable<User> query = _context.Users
                        .AsNoTracking()
                        .Include(x => x.Enterprises);

                    if (request.Login is not null)
                        query = query.Where(x => x.UserName.ToLower().Contains(request.Login.ToLower()));

                    if (request.Inn is not null)
                        query = query.Where(x => x.Inn.ToLower().Contains(request.Inn.ToLower()));

                    if (request.ExpirationDateStart is not null)
                        query = query.Where(x => x.ExpirationDate >= request.ExpirationDateStart);
                    
                    if (request.ExpirationDateEnd is not null)
                        query = query.Where(x => x.ExpirationDate <= request.ExpirationDateEnd);
                    
                    return await query.OrderBy(x => x.UserName)
                        .GetPagedAsync<User, UserViewModel>
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