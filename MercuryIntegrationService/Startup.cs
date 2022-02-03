using Core;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MercuryIntegrationService.Configurations;
using System;
using System.IO;
using Core.Application.Common.Services;
using CrystalQuartz.AspNetCore;
using Infrastructure.Options;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;

namespace MercuryIntegrationService
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            
            var builder = new ConfigurationBuilder()
                         .SetBasePath(env.ContentRootPath)
                         .AddJsonFile("appsettings.json", false, true)
                         .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                         .AddEnvironmentVariables();

            Configuration = builder.Build();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructure(Configuration);

            services.AddApplication(Configuration);

            services.AddHttpContextAccessor();

            services.AddHealthChecks();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Mercury Integration Service",
                    Version = "v1",
                    Description = "Сервис интеграции с системой \"Меркурий\""
                });

                config.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "MercuryIntegrationService.xml"));
                config.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Core.xml"));
            });

            services.AddCors();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
        }

        public static void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory,
            IOptionsMonitor<LogFolderOptions> logFolderOptions,
            ISchedulerFactory schedulerFactory)
        {
            if (logFolderOptions.CurrentValue.StoreLogs)
                loggerFactory.AddFile(logFolderOptions.CurrentValue.Folder);
            
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCustomExceptionHandler();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mercury Integration Service v1");
            });

            app.UseCors(builder =>
                builder.WithOrigins(
                        "http://localhost:8080",
                        "http://backend:80")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");
                endpoints.MapDefaultControllerRoute();
            });
            
            app.UseCrystalQuartz(() => schedulerFactory.GetScheduler().Result);
        }
    }
}
