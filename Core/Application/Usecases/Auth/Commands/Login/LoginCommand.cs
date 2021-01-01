using Core.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Usecases.Auth.Commands.Login
{
    public class LoginCommand : IRequest
    {
        public string Login { get; set; }

        public string HashedPassword { get; set; }

        public bool RememberMe { get; set; }

        private class Handler : IRequestHandler<LoginCommand>
        {
            private readonly SignInManager<User> _signInManager;

            public Handler(
                SignInManager<User> signInManager)
            {
                _signInManager = signInManager;
            }

            public async Task<Unit> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = await _signInManager
                        .PasswordSignInAsync(request.Login,
                        Encoding.UTF8.GetString(Convert.FromBase64String(request.HashedPassword)),
                        request.RememberMe, false);

                    if (!result.Succeeded)
                        throw new System.Exception("Неправильный логин и пароль");

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
