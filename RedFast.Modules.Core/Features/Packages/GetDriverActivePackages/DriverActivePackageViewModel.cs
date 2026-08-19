namespace RedFast.Modules.Core.Features.Packages.GetDriverActivePackages;

public record DriverActivePackageViewModel
(
    Guid Id,
    string TrackingCode,
    string OriginAddress,
    string DestinationAddress,
    decimal Weight,
    string CurrentStatus,
    string SenderName
    );
