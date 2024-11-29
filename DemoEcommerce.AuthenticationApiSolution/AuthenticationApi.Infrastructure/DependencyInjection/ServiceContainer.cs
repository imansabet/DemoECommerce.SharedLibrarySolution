using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService
        (this IServiceCollection services,IConfiguration config)
    {
        // Add Db Connectivity
        // Add JWT Authentication Scheme
        SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]!);

        services.AddScoped<IUser, UserRepository>();

        return services;
    }
    public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app) 
    {
        SharedServiceContainer.UseSharedPolicies(app);
        return app;
    }
}
