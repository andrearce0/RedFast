using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MediatR;

namespace RedFast.Modules.Core.Features.Packages.AssignDriver;

public static class AssignDriverEndpoint
{
    public static IEndpointRouteBuilder MapAssignDriverEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/assign", async (
            [FromRoute] Guid id,
            ClaimsPrincipal user,
            ISender mediator) =>
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
                return Results.Unauthorized();

            var command = new AssignDriverCommand(PackageId: id, UserId: userId);

            await mediator.Send(command); 

            return Results.NoContent();
        })
        .WithName("AssignDriver")
        .WithSummary("Motorista seleciona um pacote para entregar")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(p => p.RequireRole("driver"));

        return app;
    }
}
