using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Usecases.Auth.Commands.Logout
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
                catch
                {
                    throw;
                }
            }
        }
    }
}
