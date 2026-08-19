using MediatR;

namespace RedFast.Modules.Core.Features.Packages.AssignDriver;

public record AssignDriverCommand
(
    Guid PackageId,
    Guid UserId
    ) : IRequest<Unit>;
