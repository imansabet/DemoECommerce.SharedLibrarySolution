﻿using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService
        (this IServiceCollection services,IConfiguration config)
    {
        //Add Db Connection
        //Add Authentication Scheme
        SharedServiceContainer.AddSharedServices<OrderDbContext>(services, config, config["MySerilog:FileName"]!);

        services.AddScoped<IOrder, OrderRepository>();
        return services;
    }
    public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
    {
        //Regsiter Middleware
        SharedServiceContainer.UseSharedPolicies(app);
        return app;
    }
}
