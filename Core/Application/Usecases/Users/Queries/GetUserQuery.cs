using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Usecases.Users.ViewModels;
using Core.Domain.Auth;
using IdentityServer4.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Users.Queries
{
    public class GetUserQuery: IRequest<UserViewModel>
    {
        public Guid? Id { get; set; }
        
        public string UserName { get; set; }
        
        private class Handler: IRequestHandler<GetUserQuery, UserViewModel>
        {
            private readonly IMapper _mapper;
            private readonly IMisDbContext _context;

            public Handler(
                IMapper mapper,
                IMisDbContext context)
            {
                _mapper = mapper;
                _context = context;
            }
            
            public async Task<UserViewModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = new User();

                    if (request.Id.HasValue)
                        user = await _context.Users.AsNoTracking()
                                .Include(x => x.Enterprises)
                                .FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken)
                            ?? throw new Exception($@"Пользователь с идентификатором {request.Id.Value} не найден.");

                    else if (!request.UserName.IsNullOrEmpty())
                        user = await _context.Users.AsNoTracking()
                                .Include(x => x.Enterprises)
                                .FirstOrDefaultAsync(x => 
                                    x.NormalizedUserName.Equals(request.UserName.ToUpper()), cancellationToken)
                            ?? throw new Exception($@"Пользователь с именем {request.UserName} не найден.");

                    return _mapper.Map<UserViewModel>(user);
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