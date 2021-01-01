using Core.Application.Common;
using Core.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection service,
            IConfiguration configuration)
        {
            service.AddDbContext<MisDbContext>(builder =>
               builder.UseNpgsql(configuration.GetConnectionString(ConnectionStrings.PostgreSQLConnectionString),
                   b =>
                   {
                       b.MigrationsAssembly("Infrastructure");
                       b.SetPostgresVersion(12, 0);
                       b.MigrationsHistoryTable(
                           $"__MisEFMigrationsHistory",
                           "mis");
                   }));

            service.AddScoped<IMisDbContext>(provider => provider.GetService<MisDbContext>());

            service.AddMediatR(Assembly.GetExecutingAssembly());

            service.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<MisDbContext>();

            return service;
        }
    }
}
