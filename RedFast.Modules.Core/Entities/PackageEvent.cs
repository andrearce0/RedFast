using RedFast.Modules.Core.Entities.Enums;

namespace RedFast.Modules.Core.Entities;

public class PackageEvent
{
    public Guid Id { get; init; }
    public required Guid PackageId { get; init; }
    public required PackageStatus Status { get; init; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public string? Description { get; init; }
    public string? Location { get; init; }
}
