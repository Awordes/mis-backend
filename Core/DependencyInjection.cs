using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Core.Application.Common.Behaviors;
using Core.Application.Common.Options;

namespace Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMediatR(Assembly.GetExecutingAssembly());
            
            services.Configure<RoleOptions>(configuration.GetSection(nameof(RoleOptions)));

            return services;
        }
    }
}
