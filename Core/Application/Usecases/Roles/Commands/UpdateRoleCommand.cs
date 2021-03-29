using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Extensions;
using Core.Application.Common.Mapping;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.Roles.Commands
{
    public class UpdateRoleCommand: IRequest, IMapTo<Role>
    {
        [JsonIgnore]
        public Guid RoleId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        
        private class Handler: IRequestHandler<UpdateRoleCommand>
        {
            private readonly RoleManager<Role> _roleManager;
            private readonly IMapper _mapper;
            
            public Handler(
                RoleManager<Role> roleManager,
                IMapper mapper)
            {
                _roleManager = roleManager;
                _mapper = mapper;
            }
            
            public async Task<Unit> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var role = await _roleManager.FindByIdAsync(request.RoleId.ToString())
                        ?? throw new Exception($@"Роль с идентификатором {request.RoleId} не найдена.");
                    
                    _mapper.Map(request, role);

                    (await _roleManager.UpdateAsync(role)).CheckResult();
                    
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