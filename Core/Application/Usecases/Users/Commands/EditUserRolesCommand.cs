using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Extensions;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.Users.Commands
{
    public class EditUserRolesCommand: IRequest
    {
        [JsonIgnore]
        public string UserId { get; set; }

        public string[] Roles { get; set; }
        
        private class Handler: IRequestHandler<EditUserRolesCommand>
        {
            private readonly UserManager<User> _userManager;

            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }
            
            public async Task<Unit> Handle(EditUserRolesCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(request.UserId)
                        ?? throw new Exception($@"Пользователь с идентификатором {request.UserId} не найден.");
                    
                    var userRoles = await _userManager.GetRolesAsync(user);
                    
                    var addedRoles = request.Roles.Except(userRoles);
                    
                    var removedRoles = userRoles.Except(request.Roles);

                    (await _userManager.AddToRolesAsync(user, addedRoles)).CheckResult();
 
                    (await _userManager.RemoveFromRolesAsync(user, removedRoles)).CheckResult();

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