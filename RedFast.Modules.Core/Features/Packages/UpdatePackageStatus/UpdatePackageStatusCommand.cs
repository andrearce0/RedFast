using RedFast.Modules.Core.Entities.Enums;
using MediatR;

namespace RedFast.Modules.Core.Features.Packages.UpdatePackageStatus;

public record UpdatePackageStatusCommand
(
    Guid UserId,
    string UserRole,
    Guid PackageId,
    PackageStatus NewStatus,
    string? Description,
    string? Location
) : IRequest<Unit>;