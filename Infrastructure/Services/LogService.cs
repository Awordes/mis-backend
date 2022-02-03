using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using Core.Domain.Operations;
using Infrastructure.Factories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class LogService: ILogService
    {
        private readonly IMisDbContextFactory _contextFactory;

        public LogService(IMisDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Guid> StartOperation(Guid userId, OperationType operationType, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var _context = _contextFactory.Create();
                var user = await _context.Users
                               .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
                           ?? throw new Exception($@"Пользователь с идентификатором {userId} не найден.");

                var operation = new Operation { User = user, Type = operationType };
                
                _context.Operations.Add(operation);

                await _context.SaveChangesAsync(cancellationToken);
                
                return operation.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task FinishOperation(Guid operationId, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var _context = _contextFactory.Create();
                var operation = _context.Operations.Local.FirstOrDefault(x => x.Id == operationId)
                    ?? await _context.Operations.FirstOrDefaultAsync(x => x.Id == operationId, cancellationToken)
                    ?? throw new Exception($@"Операция с идентификатором {operationId} не найдена.");
                    
                operation.FinishTime = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Guid> StartVsdProcessTransaction(Guid operationId, string vsdId, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var _context = _contextFactory.Create();
                var operation =  _context.Operations.Local.FirstOrDefault(x => x.Id == operationId)
                    ?? await _context.Operations.FirstOrDefaultAsync(x => x.Id == operationId, cancellationToken)
                    ?? throw new Exception($@"Операция с идентификатором {operationId} не найдена.");

                var vsdTransaction = new VsdProcessTransaction
                {
                    Operation = operation,
                    VsdId = vsdId
                };

                _context.VsdProcessTransactions.Add(vsdTransaction);

                await _context.SaveChangesAsync(cancellationToken);

                return vsdTransaction.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task FinishVsdProcessTransaction(Guid vsdProcessTransactionId, string error,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await using var _context = _contextFactory.Create();
                var operation = _context.VsdProcessTransactions.Local.FirstOrDefault(x => x.Id == vsdProcessTransactionId)
                    ?? await _context.VsdProcessTransactions.FirstOrDefaultAsync(x => x.Id == vsdProcessTransactionId, cancellationToken)
                    ?? throw new Exception($@"Проводка по ВСД с идентификатором {vsdProcessTransactionId} не найдена.");
                    
                operation.FinishTime = DateTime.Now;

                operation.Error = error;

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}