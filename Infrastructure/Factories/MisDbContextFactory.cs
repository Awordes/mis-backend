using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Factories
{
    public class MisDbContextFactory: IMisDbContextFactory
    {
        private readonly MisDbOptions _misDbOptions;

        public MisDbContextFactory(IOptionsMonitor<MisDbOptions> misDbOptionMonitor)
        {
            _misDbOptions = misDbOptionMonitor.CurrentValue;
        }

        public MisDbContext Create()
        {
            var options = new DbContextOptionsBuilder<MisDbContext>()
                .UseNpgsql(_misDbOptions.ConnectionString,
                    b =>
                    {
                        b.MigrationsAssembly(nameof(Infrastructure));
                        b.SetPostgresVersion(_misDbOptions.PostgreSqlVersion.Major, _misDbOptions.PostgreSqlVersion.Minor);
                        b.MigrationsHistoryTable(
                            _misDbOptions.EfMigrationsHistoryTableName,
                            _misDbOptions.SchemaName);
                    }).Options;
            
            return new MisDbContext(options);
        }
    }
}