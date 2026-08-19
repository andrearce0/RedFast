using MediatR;

namespace RedFast.Modules.Core.Features.Packages.GetSenderPackages;

public record GetSenderPackagesQuery
(
    Guid UserId
    ) : IRequest<List<PackageViewModel>>;
