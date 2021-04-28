using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Options;
using Core.Application.Common.Services;
using Core.Domain;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Application.Usecases.Users.Commands
{
    public class CreateUserFromApplicantCommand: IRequest
    {
        public Guid ApplicantId { get; set; }
        
        private class Handler: IRequestHandler<CreateUserFromApplicantCommand>
        {
            private readonly IMisDbContext _context;
            private readonly IMediator _mediator;
            private readonly IPasswordService _passwordService;
            private readonly UserManager<User> _userManager;
            private readonly RoleOptions _roleOptions;

            public Handler(IMisDbContext context,
                IMediator mediator,
                IPasswordService passwordService,
                UserManager<User> userManager,
                IOptionsMonitor<RoleOptions> roleOptions)
            {
                _context = context;
                _mediator = mediator;
                _passwordService = passwordService;
                _userManager = userManager;
                _roleOptions = roleOptions.CurrentValue;
            }

            public async Task<Unit> Handle(CreateUserFromApplicantCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var applicant = await _context.Applicants
                            .FirstOrDefaultAsync(x => x.Id == request.ApplicantId, cancellationToken)
                        ?? throw new Exception($@"Заявитель с идентификатором {request.ApplicantId} не найден.");

                    if (applicant.Status == ApplicantStatus.Confirmed)
                        throw new Exception($"Заявитель {applicant.Inn} уже зарегистрирован как пользователь");

                    var passw = _passwordService.GeneratePassword(10);

                    await _mediator.Send(new CreateUserCommand
                    {
                        UserName = applicant.Inn,
                        Password = passw,
                        Inn = applicant.Inn,
                        Title = applicant.Title,
                        Contact = applicant.Email,
                        PhoneNumber = applicant.PhoneNumber
                    }, cancellationToken);
                    
                    var user = await _userManager.Users.AsNoTracking()
                               .Include(x => x.Enterprises)
                               .FirstOrDefaultAsync(x => x.NormalizedUserName.Equals(applicant.Inn), cancellationToken)
                           ?? throw new Exception($@"Пользователь с именем {applicant.Inn} не найден.");
                    
                    await _mediator.Send(new EditUserRolesCommand
                    {
                        UserId = user.Id.ToString(),
                        Roles = new [] { _roleOptions.Guest }
                    }, cancellationToken);

                    applicant.Status = ApplicantStatus.Confirmed;

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