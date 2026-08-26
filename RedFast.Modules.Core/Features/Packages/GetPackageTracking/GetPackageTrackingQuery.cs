using MediatR;

namespace RedFast.Modules.Core.Features.Packages.GetPackageTracking;

public record GetPackageTrackingQuery
(
    string TrackingCode
) : IRequest<PackageTrackingViewModel>;
