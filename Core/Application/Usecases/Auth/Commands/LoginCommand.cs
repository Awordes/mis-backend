using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Options;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
            private readonly RoleOptions _roleOptions;

            public Handler(SignInManager<User> signInManager, IOptionsMonitor<RoleOptions> roleOptions)
            {
                _signInManager = signInManager;
                _roleOptions = roleOptions.CurrentValue;
            }

            public async Task<Unit> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _signInManager.UserManager.Users.AsNoTracking().FirstOrDefaultAsync(x =>
                        x.NormalizedUserName.Equals(request.Login.ToUpper()), cancellationToken)
                        ?? throw new Exception("Неправильный логин или пароль");
                    
                    var userRoles = await _signInManager.UserManager.GetRolesAsync(user);
                    
                    //Проверяем, является ли пользователь гостем. Если да - то не проверять подписку.
                    if (!(userRoles.Count == 1 && userRoles[0].Equals(_roleOptions.Guest))
                        && DateTime.Now.CompareTo(user.ExpirationDate) > 0)
                        throw new Exception("Истёк период подписки.");
                    
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
