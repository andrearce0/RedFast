using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace RedFast.Modules.Core.Features.Packages.GetAvailablePackages;

public static class GetAvailablePackagesEndpoint
{
    public static IEndpointRouteBuilder MapGetAvailablePackages(this IEndpointRouteBuilder app)
    {
        app.MapGet("/available", async (
            ISender mediator) =>
            {
                var query = new GetAvailablePackagesQuery();

                var result = await mediator.Send(query);

                return Results.Ok(result);
            })
            .WithName("GetAvailablePackages")
            .WithSummary("Lista todos os pacotes aguardando coleta")
            .Produces<List<AvailablePackageViewModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(p => p.RequireRole("driver"));

        return app;
    }
}
