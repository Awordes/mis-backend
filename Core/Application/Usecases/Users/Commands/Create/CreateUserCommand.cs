using Core.Application.Common;
using Core.Domain.Entities.Authorization;
using MediatR;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Usecases.Users.Commands.Create
{
    public class CreateUserCommand: ICommand, IRequest
    {

        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public class Handler : IRequestHandler<CreateUserCommand>
        {
            private readonly IMisDbContext _context;

            private readonly IMediator _mediator;

            public Handler(IMisDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    byte[] salt = new byte[128 / 8];
                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(salt);
                    }

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: request.Password,
                        salt: salt,
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 9998,
                        numBytesRequested: 256 / 8));

                    var user = new User
                    {
                        Login = request.Login,
                        PasswordHash = hashed,
                        Salt = Convert.ToBase64String(salt)
                    };

                    await _context.Users.AddAsync(user, cancellationToken);

                    await _context.SaveChangesAsync(cancellationToken);

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
