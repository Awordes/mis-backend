using Core.Application.Common;
using Core.Domain.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Core.Domain;
using Core.Domain.Operations;

namespace Infrastructure
{
    internal class MisDbContext : IdentityDbContext<User, Role, Guid>, IMisDbContext
    {
        public MisDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("mis");
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Enterprise> Enterprises { get; set; }
        
        public DbSet<Operation> Operations { get; set; }
        
        public DbSet<VsdProcessTransaction> VsdProcessTransactions { get; set; }

        public DbSet<Template> Templates { get; set; }
    }
}