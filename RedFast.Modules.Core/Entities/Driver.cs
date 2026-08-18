namespace RedFast.Modules.Core.Entities;

public class Driver
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public required Guid UserId { get; init; }

    public required string Name {  get; set; }
    public required string LicenseNumber { get; set; }

    public Guid? VehicleId { get; set;  }
    public Vehicle? Vehicle { get; set; }

    public ICollection<Package> Packages { get; private set; } = new List<Package>();
}

