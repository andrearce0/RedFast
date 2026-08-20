namespace RedFast.Modules.Core.Infrastructure.Messaging;

public record PackageStatusChangedEvent
(
    Guid PackageId,
    string OldStatus,
    string NewStatus,
    DateTimeOffset OccuredAt
    );
