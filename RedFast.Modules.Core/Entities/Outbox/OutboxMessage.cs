namespace RedFast.Modules.Core.Entities.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string EventType { get; init; }
    public required string Content { get; init; }
    public DateTimeOffset OcurredOn { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcessedOn { get; private set; }
    public string? Error { get; private set; }

    public void MarkAsProcessed()
    {
        ProcessedOn = DateTimeOffset.UtcNow;
    }

    public void MarkAsFailed(string error)
    {
        Error = error;
    }
}
