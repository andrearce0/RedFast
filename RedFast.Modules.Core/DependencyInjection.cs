using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedFast.Modules.Core.Persistence;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using RedFast.Modules.Core.Features.Packages.CreatePackage;
using RedFast.Modules.Core.Behaviors;
using RedFast.Modules.Core.Features.Packages.UpdatePackageStatus;
using RedFast.Modules.Core.Features.Auth.RegisterUser;
using RedFast.Modules.Core.Features.Auth.LoginUser;
using Microsoft.Extensions.Logging;
using RedFast.Modules.Core.Features.Packages.AssignDriver;
using RedFast.Modules.Core.Features.Packages.GetSenderPackages;
using RedFast.Modules.Core.Features.Packages.GetAvailablePackages;
using RedFast.Modules.Core.Infrastructure.Messaging;

namespace RedFast.Modules.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreModule(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddDbContext<RedFastDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));

            //options.LogTo(Console.WriteLine, LogLevel.Information); // Vai imprimir o SQL no terminal
            //options.EnableSensitiveDataLogging(); // Vai mostrar os valores das variáveis (ex: o Id exato)
        });   

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IMessageBus, RabbitMqMessageBus>();

        return services;
    }

    public static IEndpointRouteBuilder MapCoreEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var packageGroup = endpoints.MapGroup("/api/packages") 
                             .WithTags("Packages");
        packageGroup.MapCreatePackageEndpoint();
        packageGroup.MapUpdatePackageStatusEndpoint();
        packageGroup.MapAssignDriverEndpoint();
        packageGroup.MapGetSenderPackages();
        packageGroup.MapGetAvailablePackages();

        var authGroup = endpoints.MapGroup("/api/auth")
                            .WithTags("Authentication");
        authGroup.MapRegisterUserEndpoint();
        authGroup.MapLoginUserEndpoint();

        return endpoints;
    }
}
