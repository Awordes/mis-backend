using Core.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Usecases.Users.Queries.CheckLogin
{
    public class CheckLoginQuery: IQuery, IRequest<bool>
    {

        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string HashedPass { get; set; }

        public class Handler : IRequestHandler<CheckLoginQuery, bool>
        {
            private readonly IMisDbContext _context;

            public Handler(IMisDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(CheckLoginQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Login == request.Login)
                        ?? throw new Exception($"User with login '{request.Login}' not found");

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: Encoding.UTF8.GetString(Convert.FromBase64String(request.HashedPass)),
                        salt: Convert.FromBase64String(user.Salt),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 9998,
                        numBytesRequested: 256 / 8));

                    return user.PasswordHash == hashed;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
