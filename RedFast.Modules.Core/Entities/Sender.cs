namespace RedFast.Modules.Core.Entities;

public class Sender
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid UserId { get; init; }

    public required string CompanyName { get; init; }

    public required string Document {  get; init; }

    public ICollection<Package> Packages { get; private set; } = new List<Package>();
}  

