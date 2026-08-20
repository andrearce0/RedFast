namespace RedFast.Modules.Notifications.Infrastructure.Messaging;

public record PackageStatusChangedEvent
(
    Guid PackageId,
    string OldStatus,
    string NewStatus,
    DateTimeOffset OcurredAt
    );
