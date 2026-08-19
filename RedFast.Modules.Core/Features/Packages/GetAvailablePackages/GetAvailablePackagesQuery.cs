using MediatR;

namespace RedFast.Modules.Core.Features.Packages.GetAvailablePackages;

public record GetAvailablePackagesQuery
() : IRequest<List<AvailablePackageViewModel>>;
