using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Extensions;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.Users.Commands
{
    public class UserChangePasswordCommand: IRequest
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [JsonIgnore]
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Новый пароль пользователя
        /// </summary>
        public string NewPassword { get; set; }
        
        private class Handler: IRequestHandler<UserChangePasswordCommand>
        {
            private readonly UserManager<User> _userManager;
            private readonly IMisDbContext _context;

            public Handler(UserManager<User> userManager,
                IMisDbContext context)
            {
                _userManager = userManager;
                _context = context;
            }
            
            public async Task<Unit> Handle(UserChangePasswordCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(request.UserId.ToString())
                        ?? throw new Exception($@"Пользователь с идентификатором {request.UserId} не найден.");

                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    
                    (await _userManager.ResetPasswordAsync(user, token, request.NewPassword)).CheckResult();
                    
                    (await _userManager.UpdateSecurityStampAsync(user)).CheckResult();

                    user.PasswordText = request.NewPassword;

                    await _context.SaveChangesAsync(cancellationToken);
                    
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