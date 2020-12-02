using Core.Application.Common;
using Core.Domain.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    class MisDbContext : DbContext, IMisDbContext
    {
        public MisDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("mis");
        }

        public DbSet<User> Users { get; set; }
    }
}