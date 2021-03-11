using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Usecases.Logging.Commands.VsdProcessTransaction;
using Core.Domain.Operations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.RequestHandlers
{
    public class VsdProcessTransactionStartHandler: IRequestHandler<VsdProcessTransactionStart, Guid>
    {
        private readonly IMisDbContext _context;
        
        public VsdProcessTransactionStartHandler(IConfiguration configuration)
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

        public async Task<Guid> Handle(VsdProcessTransactionStart request, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Database.BeginTransactionAsync(cancellationToken);
                    
                var operation = await _context.Operations
                        .FirstOrDefaultAsync(x => x.Id == request.OperationId, cancellationToken)
                    ?? throw new Exception($@"Операция с идентификатором {request.OperationId} не найдена.");

                var vsdTransaction = new VsdProcessTransaction
                {
                    Operation = operation,
                    VsdId = request.VsdId
                };

                await _context.VsdProcessTransactions.AddAsync(vsdTransaction, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                await _context.Database.CommitTransactionAsync(cancellationToken);

                return vsdTransaction.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}