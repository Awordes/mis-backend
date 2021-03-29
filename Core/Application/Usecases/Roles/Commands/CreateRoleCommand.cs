using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Extensions;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.Roles.Commands
{
    public class CreateRoleCommand: IRequest
    {
        public string Name { get; set; }
        
        private class Handler: IRequestHandler<CreateRoleCommand>
        {
            private readonly RoleManager<Role> _roleManager;

            public Handler(RoleManager<Role> roleManager)
            {
                _roleManager = roleManager;
            }
            
            public async Task<Unit> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    (await _roleManager.CreateAsync(new Role(request.Name))).CheckResult();
                    
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