using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Logging.Operations
{
    public class OperationFinish: IRequest
    {
        public Guid OperationId { get; set; }
        
        private class Handler: IRequestHandler<OperationFinish>
        {
            private readonly IMisDbContext _context;
            
            public Handler(IMisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(OperationFinish request, CancellationToken cancellationToken)
            {
                try
                {
                    var operation = await _context.Operations
                            .FirstOrDefaultAsync(x => x.Id == request.OperationId, cancellationToken)
                        ?? throw new Exception($@"Операция с идентификатором {request.OperationId} не найдена.");
                    
                    operation.FinishTime = DateTime.Now;

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