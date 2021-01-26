using Core.Application.Common;
using Core.Domain.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure
{
    class MisDbContext : IdentityDbContext<User, Role, Guid>, IMisDbContext
    {
        public MisDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("mis");
            base.OnModelCreating(modelBuilder);
        }
    }
}