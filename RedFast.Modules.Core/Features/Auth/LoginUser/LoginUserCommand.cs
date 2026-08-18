using MediatR;

namespace RedFast.Modules.Core.Features.Auth.LoginUser;

public record LoginUserCommand
(
    string Email,
    string Password
) : IRequest<string>;
