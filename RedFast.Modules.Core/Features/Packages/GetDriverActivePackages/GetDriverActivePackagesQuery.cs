using MediatR;

namespace RedFast.Modules.Core.Features.Packages.GetDriverActivePackages;

public record GetDriverActivePackagesQuery
(
    Guid UserId
    ) : IRequest<List<DriverActivePackageViewModel>>;
