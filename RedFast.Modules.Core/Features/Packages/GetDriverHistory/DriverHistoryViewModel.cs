namespace RedFast.Modules.Core.Features.Packages.GetDriverHistory;

public record DriverHistoryViewModel
(
    Guid PackageId,
    string OriginAddress,
    string DestinationAddress,
    string Status,
    decimal Weight
);
