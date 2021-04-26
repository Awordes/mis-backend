using Core.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain;
using Core.Domain.Operations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Core.Application.Common
{
    public interface IMisDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        
        DatabaseFacade Database { get; }

        DbSet<User> Users { get; set; }
        
        DbSet<Enterprise> Enterprises { get; set; }
        
        DbSet<Operation> Operations { get; set; }
        
        DbSet<VsdProcessTransaction> VsdProcessTransactions { get; set; }
        
        DbSet<Template> Templates { get; set; }
    }
}
