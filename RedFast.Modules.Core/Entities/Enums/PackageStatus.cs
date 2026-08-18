namespace RedFast.Modules.Core.Entities.Enums;

public enum PackageStatus
{
    Created = 1,          // Pacote gerado pelo remetente
    AwaitingPickup = 2,   // Motorista aceitou a corrida e está indo buscar
    Collected = 3,        // Motorista pegou o pacote e está indo entregar
    Delivered = 4,        // Entregue com sucesso no destino
    DeliveryFailed = 5,   // Tentativa falhou (endereço não achado, etc.)
    Cancelled = 6         // Cancelado antes da coleta
}
