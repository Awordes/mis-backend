using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Usecases.Enterprises.ViewModels;
using Core.Application.Usecases.Users.ViewModels;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Users.Queries
{
    public class GetUserEnterprisesQuery: IRequest<UserEnterprisesViewModel>
    {
        public Guid UserId { get; set; }
        
        private class Handler: IRequestHandler<GetUserEnterprisesQuery, UserEnterprisesViewModel>
        {
            private readonly UserManager<User> _userManager;
            private readonly IMapper _mapper;
            
            public Handler(UserManager<User> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }
            
            public async Task<UserEnterprisesViewModel> Handle(GetUserEnterprisesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userManager.Users.AsNoTracking().Include(x => x.Enterprises)
                            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                        ?? throw new Exception($@"Пользователь с идентификатором {request.UserId} не найден.");

                    return new UserEnterprisesViewModel
                    {
                        Enterprises = _mapper.Map<ICollection<EnterpriseViewModel>>(user.Enterprises)
                    };
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