using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace RedFast.Modules.Core.Features.Packages.CreatePackage;

public static class CreatePackageEndpoint
{
    public static IEndpointRouteBuilder MapCreatePackageEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
            [FromBody] CreatePackageRequest request, 
            ClaimsPrincipal user,
            ISender mediator) =>
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
                return Results.Unauthorized();

            var command = new CreatePackageCommand(userId, request.OriginAddress, request.DestinationAddress, request.Weight);

            var packageId = await mediator.Send(command);

            return Results.Created($"/api/packages/{packageId}", new { Id = packageId });
        })
        .WithName("CreatePackage")
        .WithSummary("Cria um novo pacote para envio")
        .WithDescription("Registra uma nova encomenda com um novo ID e retorna o ID do pacote criado.")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();

        return app;
    }
}

public record CreatePackageRequest(decimal Weight, string OriginAddress, string DestinationAddress);
