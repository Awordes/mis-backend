using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Usecases.Users.ViewModels;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Users.Queries
{
    public class GetUserRolesQuery: IRequest<UserRolesViewModel>
    {
        public Guid UserId { get; set; }
        
        private class Handler: IRequestHandler<GetUserRolesQuery, UserRolesViewModel>
        {
            private readonly UserManager<User> _userManager;

            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }
            
            public async Task<UserRolesViewModel> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userManager.Users.AsNoTracking()
                           .Include(x => x.Enterprises)
                           .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                       ?? throw new Exception($@"Пользователь с идентификатором {request.UserId} не найден.");

                    return new UserRolesViewModel
                    {
                        Roles = await _userManager.GetRolesAsync(user)
                    };
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