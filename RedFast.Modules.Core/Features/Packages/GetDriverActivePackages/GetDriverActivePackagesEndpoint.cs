using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace RedFast.Modules.Core.Features.Packages.GetDriverActivePackages;

public static class GetDriverActivePackagesEndpoint
{
    public static IEndpointRouteBuilder MapGetDriverActivePackages(this IEndpointRouteBuilder app)
    {
        app.MapGet("/my-deliveries", async (ClaimsPrincipal user,
            ISender mediator) =>
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!Guid.TryParse(userIdString, out var userId))
                return Results.Unauthorized();

            var query = new GetDriverActivePackagesQuery(userId);
            var result = await mediator.Send(query);

            return Results.Ok(result);
        })
        .WithName("GetDriverActivePackages")
        .WithSummary("Lista as entregas em andamento do motorista autenticado")
        .Produces<List<DriverActivePackageViewModel>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(p => p.RequireRole("driver"));

        return app;
    }
}
