using RedFast.Modules.Core.Entities.Enums;

namespace RedFast.Modules.Core.Entities;

public class Package
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string TrackingCode { get; init; }
    public required string OriginAddress { get; set; }
    public required string DestinationAddress { get; set; }
    public required decimal Weight { get; init; }

    public PackageStatus CurrentStatus { get; private set; } = PackageStatus.Created;

    public required Guid SenderId { get; init; }
    public Sender? Sender { get; init; }

    public Guid? DriverId { get; private set; }
    public Driver? Driver { get; private set; }

    private readonly List<PackageEvent> _events = new();
    public IReadOnlyCollection<PackageEvent> Events => _events.AsReadOnly();

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public void AssignDriver(Guid driverId)
    {
        if (DriverId.HasValue && DriverId.Value != Guid.Empty)
            throw new InvalidOperationException("Este pacote já tem um motorista designado");

        if (CurrentStatus != PackageStatus.Created && CurrentStatus != PackageStatus.AwaitingPickup)
            throw new InvalidOperationException("O status atual do pacote não permite a atribuição de motorista");

        DriverId = driverId;
        CurrentStatus = PackageStatus.AwaitingPickup;

        _events.Add(new PackageEvent
        {
            PackageId = Id,
            Status = CurrentStatus,
            Description = "Pacote atribuído a um motorista e aguardando a coleta",
            Location = null
        });
    }

    public void UpdateStatus(PackageStatus newStatus, string? description = null, string? location = null)
    {
        bool isValidTransition = CurrentStatus switch
        {
            PackageStatus.Created => newStatus == PackageStatus.AwaitingPickup,

            PackageStatus.AwaitingPickup => newStatus == PackageStatus.Collected,

            PackageStatus.Collected => newStatus == PackageStatus.Delivered || newStatus == PackageStatus.DeliveryFailed,

            PackageStatus.Delivered => false,
            PackageStatus.DeliveryFailed => false,

            _ => false
        };

        if (!isValidTransition)
            throw new InvalidOperationException($"Transição de status inválida: não é possível alterar de {CurrentStatus} para {newStatus}");

        CurrentStatus = newStatus;

        _events.Add(new PackageEvent
        {
            PackageId = this.Id,
            Status = newStatus,
            Description = description,
            Location = location
        });
    }
}

