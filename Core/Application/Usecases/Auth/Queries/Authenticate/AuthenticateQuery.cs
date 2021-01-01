using Core.Application.Usecases.Auth.ViewModels;
using Core.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Usecases.Auth.Queries.Authenticate
{
    public class AuthenticateQuery: IRequest<AuthenticationViewModel>
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }

        private class Handler : IRequestHandler<AuthenticateQuery, AuthenticationViewModel>
        {
            private readonly SignInManager<User> _signInManager;

            public Handler(
                SignInManager<User> signInManager)
            {
                _signInManager = signInManager;
            }

            public async Task<AuthenticationViewModel> Handle(AuthenticateQuery request, CancellationToken cancellationToken)
            {
                var result = await _signInManager.PasswordSignInAsync(request.Login, request.Password, request.RememberMe, false);
                return new AuthenticationViewModel();
            }
        }
    }
}
