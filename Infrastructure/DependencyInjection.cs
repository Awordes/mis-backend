﻿using Core.Application.Common;
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

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<MisDbContext>(builder =>
               builder.UseNpgsql(configuration.GetConnectionString(ConnectionStrings.PostgreSQLConnectionString),
                   b =>
                   {
                       b.MigrationsAssembly("Infrastructure");
                       b.SetPostgresVersion(12, 0);
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
                .AddEntityFrameworkStores<MisDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(10);
                options.Cookie.Name = "MercuryIntegrationService";

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddScoped<MercuryService>();
            services.AddScoped<IMercuryService>(provider => provider.GetService<MercuryService>());

            return services;
        }
    }
}
