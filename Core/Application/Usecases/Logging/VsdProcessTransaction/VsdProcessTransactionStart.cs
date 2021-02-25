using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Logging.VsdProcessTransaction
{
    public class VsdProcessTransactionStart: IRequest<Guid>
    {
        public Guid OperationId { get; set; }

        public string VsdId { get; set; }
        
        private class Handler: IRequestHandler<VsdProcessTransactionStart, Guid>
        {
            private readonly IMisDbContext _context;
            
            public Handler(IMisDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(VsdProcessTransactionStart request, CancellationToken cancellationToken)
            {
                try
                {
                    var operation = await _context.Operations
                            .FirstOrDefaultAsync(x => x.Id == request.OperationId, cancellationToken)
                        ?? throw new Exception($@"Операция с идентификатором {request.OperationId} не найдена.");

                    var vsdTransaction = new Domain.Operations.VsdProcessTransaction
                    {
                        Operation = operation,
                        VsdId = request.VsdId
                    };

                    await _context.VsdProcessTransactions.AddAsync(vsdTransaction, cancellationToken);

                    await _context.SaveChangesAsync(cancellationToken);

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
}