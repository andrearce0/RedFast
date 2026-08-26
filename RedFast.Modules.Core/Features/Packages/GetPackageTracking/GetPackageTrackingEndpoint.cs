using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace RedFast.Modules.Core.Features.Packages.GetPackageTracking;

public static class GetPackageTrackingEndpoint
{
    public static IEndpointRouteBuilder MapGetPackageTracking(this IEndpointRouteBuilder app)
    {
        app.MapGet("/tracking/{trackingCode}", async (
            [FromRoute] string trackingCode,
            ISender mediator) =>
        {
            var query = new GetPackageTrackingQuery(trackingCode);

            var result =  await mediator.Send(query);

            if (result == null)
                return Results.NotFound(new { Message = "Código de rastreio inválido ou não encontrado." });

            return Results.Ok(result);
        })
        .WithName("GetPackageTracking")
        .WithSummary("Localiza um pacote por seu código de rastreio e lista informações sobre ele")
        .Produces<PackageTrackingViewModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
