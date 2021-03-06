using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Usecases.Roles.ViewModels;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Roles.Queries
{
    public class GetRolesQuery: IRequest<RoleListViewModel>
    {
        private class Handler: IRequestHandler<GetRolesQuery, RoleListViewModel>
        {
            private readonly RoleManager<Role> _roleManager;
            private readonly IMapper _mapper;
            
            public Handler(RoleManager<Role> roleManager, IMapper mapper)
            {
                _roleManager = roleManager;
                _mapper = mapper;
            }
            
            public async Task<RoleListViewModel> Handle(GetRolesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    return new RoleListViewModel
                    {
                        Roles = _mapper.Map<ICollection<RoleViewModel>>
                            (await _roleManager.Roles.ToListAsync(cancellationToken))
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