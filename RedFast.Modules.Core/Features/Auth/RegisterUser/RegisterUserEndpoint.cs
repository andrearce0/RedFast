using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace RedFast.Modules.Core.Features.Auth.RegisterUser;

public static class RegisterUserEndpoint
{
    public static IEndpointRouteBuilder MapRegisterUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async (RegisterUserCommand command, ISender mediator) =>
        {
            var userId = mediator.Send(command);

            return Results.Ok(new { UserId = userId, Message = "Usuário registrado com sucesso." });
        })
        .WithName("RegisterUser")
        .WithSummary("Registra um novo usuário no sistema.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
