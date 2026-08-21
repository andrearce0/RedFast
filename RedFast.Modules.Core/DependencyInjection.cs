using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedFast.Modules.Core.Behaviors;
using RedFast.Modules.Core.Features.Auth.LoginUser;
using RedFast.Modules.Core.Features.Auth.RegisterUser;
using RedFast.Modules.Core.Features.Packages.AssignDriver;
using RedFast.Modules.Core.Features.Packages.CreatePackage;
using RedFast.Modules.Core.Features.Packages.GetAvailablePackages;
using RedFast.Modules.Core.Features.Packages.GetDriverActivePackages;
using RedFast.Modules.Core.Features.Packages.GetSenderPackages;
using RedFast.Modules.Core.Features.Packages.UpdatePackageStatus;
using RedFast.Modules.Core.Infrastructure.Messaging;
using RedFast.Modules.Core.Persistence;
using System.Reflection;

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
        packageGroup.MapGetDriverActivePackages();

        var authGroup = endpoints.MapGroup("/api/auth")
                            .WithTags("Authentication");
        authGroup.MapRegisterUserEndpoint();
        authGroup.MapLoginUserEndpoint();

        return endpoints;
    }
}
