using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Usecases.Logging.Commands.Operations;
using Core.Domain.Operations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.RequestHandlers
{
    public class OperationStartHandler: IRequestHandler<OperationStart, Guid>
    {
        private readonly IMisDbContext _context;
        
        public OperationStartHandler(IConfiguration configuration)
        {
            var optionBuilder = new DbContextOptionsBuilder<MisDbContext>();
            optionBuilder.UseNpgsql(configuration.GetConnectionString(ConnectionStrings.PostgreSqlConnectionString),
                b =>
                {
                    b.MigrationsAssembly("Infrastructure");
                    b.SetPostgresVersion(12, 0);
                    b.MigrationsHistoryTable(
                        $"__MisEFMigrationsHistory",
                        "mis");
                });

            _context = new MisDbContext(optionBuilder.Options);
        }
        
        public async Task<Guid> Handle(OperationStart request, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Database.BeginTransactionAsync(cancellationToken);
                
                var user = await _context.Users
                       .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                   ?? throw new Exception($@"Пользователь с идентификатором {request.UserId} не найден.");

                var operation = new Operation { User = user, Type = request.Type };

                await _context.Operations.AddAsync(operation, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                await _context.Database.CommitTransactionAsync(cancellationToken);

                return operation.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}