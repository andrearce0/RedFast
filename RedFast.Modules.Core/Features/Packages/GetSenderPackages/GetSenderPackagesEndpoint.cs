using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace RedFast.Modules.Core.Features.Packages.GetSenderPackages;

public static class GetSenderPackagesEndpoint
{
    public static IEndpointRouteBuilder MapGetSenderPackages(this IEndpointRouteBuilder app)
    {
        app.MapGet("my-packages", async (
            ClaimsPrincipal user,
            ISender mediator) =>
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
                return Results.Unauthorized();

            var query = new GetSenderPackagesQuery(UserId: userId);

            var result = await mediator.Send(query);

            return Results.Ok(result);
        })
        .WithName("GetSenderPackages")
        .WithSummary("Lista todos os pacotes criados pelo remetente autenticado")
        .Produces <List<PackageViewModel>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(p => p.RequireRole("sender"));

        return app;
    }
}
