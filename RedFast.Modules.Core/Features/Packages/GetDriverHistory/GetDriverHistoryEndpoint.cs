using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace RedFast.Modules.Core.Features.Packages.GetDriverHistory;

public static class GetDriverHistoryEndpoint
{
    public static IEndpointRouteBuilder MapGetDriverHistory(this IEndpointRouteBuilder app)
    {
        app.MapGet("/my-history", async (
            ClaimsPrincipal user,
            ISender mediator) =>
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!Guid.TryParse(userIdString, out var userId))
                return Results.Unauthorized();

            var query = new GetDriverHistoryQuery(userId);

            var result = await mediator.Send(query);

            return Results.Ok(result);
        })
        .WithName("GetDriverHistory")
        .WithSummary("Histórico de entregas concluídas pelo motorista autenticado")
        .Produces<List<DriverHistoryViewModel>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(p => p.RequireRole("driver"));

        return app;
    }
}
