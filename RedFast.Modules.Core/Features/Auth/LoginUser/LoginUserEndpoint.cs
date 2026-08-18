using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace RedFast.Modules.Core.Features.Auth.LoginUser;

public static class LoginUserEndpoint
{
    public static IEndpointRouteBuilder MapLoginUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (LoginUserCommand request, ISender mediator) =>
        {
            try
            {
                var token = await mediator.Send(request);
                return Results.Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(
                    title: "Falha na Autenticação",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status401Unauthorized
                );
            }
        })
        .WithName("LoginUser")
        .WithSummary("Autentica um usuário e retorna um token JWT.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
