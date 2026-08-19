using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using RedFast.Modules.Core.Entities.Enums;
using Microsoft.AspNetCore.Builder;
using System.Security.Claims;

namespace RedFast.Modules.Core.Features.Packages.UpdatePackageStatus;

public static class UpdatePackageStatusEndpoint
{
    public static IEndpointRouteBuilder MapUpdatePackageStatusEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/status", async (
            [FromRoute] Guid id,
            [FromBody] UpdatePackageStatusRequest request,
            ClaimsPrincipal user,
            ISender mediator) =>
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(!Guid.TryParse(userIdString, out Guid userId))
                return Results.Unauthorized();
             
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

            var command = new UpdatePackageStatusCommand(
                userId,
                roleClaim!,
                id,
                request.NewStatus,
                request.Description,
                request.Location);

            await mediator.Send(command);

            return Results.NoContent();
        })  
        .WithName("UpdatePackageStatus")
        .WithSummary("Atualiza o status de um pacote existente.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization(p => p.RequireRole("admin", "driver"));

        return app;
    }
}

public record UpdatePackageStatusRequest(
    PackageStatus NewStatus,
    string? Description,
    string? Location);
