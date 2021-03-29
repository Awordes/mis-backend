using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Usecases.Logging.Commands.Operations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.RequestHandlers
{
    public class OperationFinishHandler: IRequestHandler<OperationFinish>
    {
        private readonly IMisDbContext _context;
        
        public OperationFinishHandler(IConfiguration configuration)
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
        
        public async Task<Unit> Handle(OperationFinish request, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Database.BeginTransactionAsync(cancellationToken);
                    
                var operation = await _context.Operations
                        .FirstOrDefaultAsync(x => x.Id == request.OperationId, cancellationToken)
                    ?? throw new Exception($@"Операция с идентификатором {request.OperationId} не найдена.");
                    
                operation.FinishTime = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);

                await _context.Database.CommitTransactionAsync(cancellationToken);
                    
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