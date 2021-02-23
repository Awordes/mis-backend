using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Usecases.Users.Queries.GetUser;
using Core.Application.Usecases.Users.ViewModels;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.Users.Queries.GetCurrentUser
{
    public class GetCurrentUserQuery: IRequest<UserViewModel>
    {
        private class Handler: IRequestHandler<GetCurrentUserQuery, UserViewModel>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;
            private readonly UserManager<User> _userManager;

            public Handler(
                IMediator mediator,
                UserManager<User> userManager,
                IHttpContextAccessor httpContextAccessor)
            {
                _mediator = mediator;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<UserViewModel> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;

                    var user = await _userManager.FindByNameAsync(userName)
                        ?? throw new Exception($@"Пользователь с именем {userName} не найден.");
                    
                    return await _mediator.Send(new GetUserQuery
                    {
                        UserName = userName
                    }, cancellationToken);
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