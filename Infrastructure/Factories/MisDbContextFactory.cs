using Core.Application.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Factories
{
    public class MisDbContextFactory: IMisDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public MisDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MisDbContext Create()
        {
            var options = new DbContextOptionsBuilder<MisDbContext>()
                .UseNpgsql(_configuration.GetConnectionString(ConnectionStrings.PostgreSqlConnectionString),
                    b =>
                    {
                        b.MigrationsAssembly(nameof(Infrastructure));
                        b.SetPostgresVersion(12, 6);
                        b.MigrationsHistoryTable(
                            $"__MisEFMigrationsHistory",
                            "mis");
                    }).Options;
            
            return new MisDbContext(options);
        }
    }
}