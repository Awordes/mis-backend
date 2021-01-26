using Core.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Common
{
    public interface IMisDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        DbSet<User> Users { get; set; }
    }
}
