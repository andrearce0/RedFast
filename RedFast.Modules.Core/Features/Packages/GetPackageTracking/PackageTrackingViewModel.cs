namespace RedFast.Modules.Core.Features.Packages.GetPackageTracking;

public record PackageTrackingViewModel
(
    string TrackingCode,
    string Status,
    string OriginAddress,
    string DestinationAddress,  
    
    List<TrackingEventViewModel> EventList
);

public record TrackingEventViewModel
(
    string Status,
    string? Description,
    string? Location,
    DateTimeOffset Timestamp
);
