using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Extensions;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Users.Commands
{
    public class DeleteUserCommand: IRequest
    {
        public Guid UserId { get; set; }
        
        private class Handler: IRequestHandler<DeleteUserCommand>
        {
            private readonly UserManager<User> _userManager;

            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userManager.Users.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                        ?? throw new Exception($@"Пользователь с идентификатором {request.UserId} не найден.");

                    (await _userManager.DeleteAsync(user)).CheckResult();
                    
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