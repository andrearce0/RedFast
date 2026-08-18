using MediatR;

namespace RedFast.Modules.Core.Features.Auth.RegisterUser;

public record RegisterUserCommand
(
    string Name,
    string Document,
    string Email,
    string Password
) : IRequest<Guid>;
