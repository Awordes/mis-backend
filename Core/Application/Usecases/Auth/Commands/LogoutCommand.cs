using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.Auth.Commands
{
    public class LogoutCommand: IRequest
    {
        private class Handler : IRequestHandler<LogoutCommand>
        {
            private readonly SignInManager<User> _signInManager;

            public Handler(
                SignInManager<User> signInManager)
            {
                _signInManager = signInManager;
            }

            public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    await _signInManager.SignOutAsync();

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
