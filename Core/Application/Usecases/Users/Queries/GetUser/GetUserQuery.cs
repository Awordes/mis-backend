using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Usecases.Users.ViewModels;
using Core.Domain.Auth;
using IdentityServer4.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.Users.Queries.GetUser
{
    public class GetUserQuery: IRequest<UserViewModel>
    {
        public Guid? Id { get; set; }
        
        public string UserName { get; set; }
        
        private class Handler: IRequestHandler<GetUserQuery, UserViewModel>
        {
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;

            public Handler(
                IMapper mapper,
                UserManager<User> userManager)
            {
                _mapper = mapper;
                _userManager = userManager;
            }
            
            public async Task<UserViewModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = new User();

                    if (request.Id.HasValue)
                        user = await _userManager.FindByIdAsync(request.Id.Value.ToString())
                            ?? throw new Exception($@"Пользователь с идентификатором {request.Id.Value} не найден.");

                    else if (!request.UserName.IsNullOrEmpty())
                        user = await _userManager.FindByNameAsync(request.UserName)
                            ?? throw new Exception($@"Пользователь с именем {request.UserName} не найден.");

                    return _mapper.Map<UserViewModel>(user);
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