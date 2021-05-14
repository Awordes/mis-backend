using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.Common.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMisDbContext _context;

        public TransactionBehavior(IMisDbContext context)
        {
            _context = context;
        }

        public async Task<TResponse> Handle(TRequest request,
            CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var nullTransaction = false;
            if (_context.Database.CurrentTransaction is null)
            {
                await _context.Database.BeginTransactionAsync(cancellationToken);
                nullTransaction = true;
            }

            var response = await next();

            if (nullTransaction) await _context.Database.CommitTransactionAsync(cancellationToken);

            return response;
        }
    }

}