using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Domain.Operations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Logging.Commands.Operations
{
    public class OperationStart: IRequest<Guid>
    {
        public Guid UserId { get; set; }

        public OperationType Type { get; set; }
        
        private class Handler: IRequestHandler<OperationStart, Guid>
        {
            private readonly IMisDbContext _context;
            
            public Handler(IMisDbContext context)
            {
                _context = context;
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
}