﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Usecases.Users.ViewModels;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.Users.Queries
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
                    var user = await _userManager
                            .FindByNameAsync(_httpContextAccessor.HttpContext?.User.Identity?.Name)
                        ?? throw new Exception($@"Пользователь с именем не найден.");
                    
                    return await _mediator.Send(new GetUserQuery
                    {
                        UserName = user.UserName
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