namespace RedFast.Modules.Core.Features.Packages.GetAvailablePackages;

public record AvailablePackageViewModel
(
    Guid Id,
    string TrackingCode,
    string OriginAddress,
    string DestinationAddress,
    decimal Weight,
    DateTimeOffset CreatedAt
    );
