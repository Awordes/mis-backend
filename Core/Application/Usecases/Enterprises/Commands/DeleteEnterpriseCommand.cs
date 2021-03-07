using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Enterprises.Commands
{
    public class DeleteEnterpriseCommand: IRequest
    {
        public Guid EnterpriseId { get; set; }
        
        private class Handler: IRequestHandler<DeleteEnterpriseCommand>
        {
            private readonly IMisDbContext _context;

            public Handler(IMisDbContext context)
            {
                _context = context;
            }
            
            public async Task<Unit> Handle(DeleteEnterpriseCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = await _context.Enterprises
                            .FirstOrDefaultAsync(x => x.Id == request.EnterpriseId, cancellationToken)
                        ?? throw new Exception($@"Предприятие с идентификатором {request.EnterpriseId} не найдено.");

                    _context.Enterprises.Remove(entity);

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