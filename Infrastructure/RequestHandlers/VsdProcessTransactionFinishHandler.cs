using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Usecases.Logging.Commands.VsdProcessTransaction;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.RequestHandlers
{
    public class VsdProcessTransactionFinishHandler: IRequestHandler<VsdProcessTransactionFinish>
    {
        private readonly IMisDbContext _context;

        public VsdProcessTransactionFinishHandler(IConfiguration configuration)
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
        
        public async Task<Unit> Handle(VsdProcessTransactionFinish request, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Database.BeginTransactionAsync(cancellationToken);
                    
                var operation = await _context.VsdProcessTransactions
                        .FirstOrDefaultAsync(x => x.Id == request.VsdProcessTransactionId, cancellationToken)
                    ?? throw new Exception($@"Проводка по ВСД с идентификатором {request.VsdProcessTransactionId} не найдена.");
                    
                operation.FinishTime = DateTime.Now;

                operation.Error = request.Error;

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