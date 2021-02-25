using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Logging.VsdProcessTransaction
{
    public class VsdProcessTransactionFinish: IRequest
    {
        public Guid VsdProcessTransactionId { get; set; }

        public string Error { get; set; }
        
        private class Handler: IRequestHandler<VsdProcessTransactionFinish>
        {
            private readonly IMisDbContext _context;
            
            public Handler(IMisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(VsdProcessTransactionFinish request, CancellationToken cancellationToken)
            {
                try
                {
                    await _context.Database.BeginTransactionAsync(cancellationToken);
                    
                    var operation = await _context.VsdProcessTransactions
                            .FirstOrDefaultAsync(x => x.Id == request.VsdProcessTransactionId, cancellationToken)
                        ?? throw new Exception(
                            $@"Проводка по ВСД с идентификатором {request.VsdProcessTransactionId} не найдена.");
                    
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
}