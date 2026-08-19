namespace RedFast.Modules.Core.Features.Packages.GetSenderPackages;

public record PackageViewModel
(
    Guid Id,
    string TrackingCode,
    string OriginAddress,
    string DestinationAddress,
    decimal Weight,
    string CurrentStatus,
    DateTimeOffset CreatedAt
    );
