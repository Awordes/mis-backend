using Core;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Presentation.Configurations;
using System;
using System.IO;

namespace Presentation
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

                config.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Presentation.xml"));
                config.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Core.xml"));
            });

            services.AddCors();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
                        "http://my.server:8080")
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
        }
    }
}
