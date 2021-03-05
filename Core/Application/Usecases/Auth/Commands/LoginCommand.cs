using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.Auth.Commands
{
    public class LoginCommand : IRequest
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }

        private class Handler : IRequestHandler<LoginCommand>
        {
            private readonly SignInManager<User> _signInManager;

            public Handler(SignInManager<User> signInManager)
            {
                _signInManager = signInManager;
            }

            public async Task<Unit> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = await _signInManager
                        .PasswordSignInAsync(request.Login,
                        request.Password,
                        request.RememberMe, false);

                    if (!result.Succeeded)
                        throw new Exception("Неправильный логин или пароль");

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
