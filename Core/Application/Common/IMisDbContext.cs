using Core.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Operations;

namespace Core.Application.Common
{
    public interface IMisDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        DbSet<User> Users { get; set; }
        
        DbSet<Enterprise> Enterprises { get; set; }
        
        DbSet<Operation> Operations { get; set; }
        
        DbSet<VsdProcessTransaction> VsdProcessTransactions { get; set; }
    }
}
