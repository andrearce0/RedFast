namespace RedFast.Modules.Core.Entities;

public class Vehicle
{
    public Guid Id { get; init; }
    public required string LicensePlate { get; set; }
    public required string Model { get; set; }
    public required decimal MaxCapacityKg { get; set; }
}
