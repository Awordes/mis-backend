using Core.Application.Common;
using Core.Application.Common.Services;
using Core.Domain.Auth;
using Infrastructure.Integrations.Mercury;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Infrastructure.Options;
using Infrastructure.Services;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<MisDbContext>(builder =>
               builder.UseNpgsql(configuration.GetConnectionString(ConnectionStrings.PostgreSqlConnectionString),
                   b =>
                   {
                       b.MigrationsAssembly("Infrastructure");
                       b.SetPostgresVersion(12, 6);
                       b.MigrationsHistoryTable(
                           $"__MisEFMigrationsHistory",
                           "mis");
                   }));

            services.AddScoped<IMisDbContext>(provider => provider.GetService<MisDbContext>());

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequiredLength = 10;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                })
                .AddEntityFrameworkStores<MisDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(10);
                options.Cookie.Name = "MercuryIntegrationService";
            });
            
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;   
            });

            services.AddScoped<IMercuryService, MercuryService>();
            services.AddScoped<IFileService, FileService>();

            services.Configure<MercuryOptions>(configuration.GetSection(nameof(MercuryOptions)));
            services.Configure<MercuryFileOptions>(configuration.GetSection(nameof(MercuryFileOptions)));
            
            return services;
        }
    }
}
