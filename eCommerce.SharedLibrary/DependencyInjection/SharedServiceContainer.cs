using eCommerce.SharedLibrary.MiddleWare;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection;

public static class SharedServiceContainer
{
    public static IServiceCollection AddSharedServices<TContext>
        (this IServiceCollection services
        , IConfiguration config
        ,string fileName) where TContext : DbContext
    {
        // Add Generic Database Context
        services.AddDbContext<TContext>(option
            => option.UseSqlServer
            ( config.GetConnectionString("eCommerceConnection"), sqlserverOption => 
              sqlserverOption.EnableRetryOnFailure ()));

        //configure serilog logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(path: $"{fileName}-.text",
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
            outputTemplate: "{Timestamp:yyyy-MM-dd Hh:mm:ss.fff zzz} [{Level:u3}] {message:lj}{NewLine}{Exception}"
            , rollingInterval: RollingInterval.Day)
            .CreateLogger();

        //Add JWT Authentication Scheme
        JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);
        
        return services;
    }
    public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
    {
        //use Global Exception
        app.UseMiddleware<GlobalException>();
        //regiister middleware to block all outsiders api calls
        app.UseMiddleware<ListenOnlyToApiGateway>();
        return app;

    }
}
