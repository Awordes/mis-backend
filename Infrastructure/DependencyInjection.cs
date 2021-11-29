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
using Infrastructure.Factories;
using Infrastructure.Options;
using Infrastructure.Services;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MercuryOptions>(configuration.GetSection(nameof(MercuryOptions)));
            services.Configure<MercuryFileOptions>(configuration.GetSection(nameof(MercuryFileOptions)));

            var misDbOptionsConfig = configuration.GetSection(MisDbOptions.SectionName);
            services.Configure<MisDbOptions>(misDbOptionsConfig);
            var misDbOptions = new MisDbOptions();
            misDbOptionsConfig.Bind(misDbOptions);
            
            var identityOptionsConfig = configuration.GetSection(IdentityConfigurationOptions.SectionName);
            services.Configure<IdentityConfigurationOptions>(identityOptionsConfig);
            var identityOptions = new IdentityConfigurationOptions();
            identityOptionsConfig.Bind(identityOptions);
            
            services.AddDbContext<MisDbContext>(builder =>
               builder.UseNpgsql(misDbOptions.ConnectionString,
                   b =>
                   {
                       b.MigrationsAssembly(nameof(Infrastructure));
                       b.SetPostgresVersion(misDbOptions.PostgreSqlVersion.Major, misDbOptions.PostgreSqlVersion.Minor);
                       b.MigrationsHistoryTable(
                           misDbOptions.EfMigrationsHistoryTableName,
                           misDbOptions.SchemaName);
                   }));

            services.AddScoped<IMisDbContext>(provider => provider.GetService<MisDbContext>());

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequiredLength = identityOptions.Password.RequiredLength;
                    options.Password.RequireLowercase = identityOptions.Password.RequireLowercase;
                    options.Password.RequireUppercase = identityOptions.Password.RequireUppercase;
                    options.Password.RequireNonAlphanumeric = identityOptions.Password.RequireNonAlphanumeric;
                    options.Password.RequireDigit = identityOptions.Password.RequireDigit;
                })
                .AddEntityFrameworkStores<MisDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = identityOptions.Cookie.HttpOnly;
                options.SlidingExpiration = identityOptions.Cookie.SlidingExpiration;
                options.ExpireTimeSpan = TimeSpan.FromHours(identityOptions.Cookie.ExpireTimeSpanHoursCount);
                options.Cookie.Name = identityOptions.Cookie.Name;
            });
            
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;   
            });

            services.AddScoped<IMercuryService, MercuryService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IMisDbContextFactory, MisDbContextFactory>();
            
            return services;
        }
    }
}
