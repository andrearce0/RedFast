using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Entities;
using RedFast.Modules.Core.Persistence;

namespace RedFast.Modules.Core.Features.Packages.CreatePackage;

public class CreatePackageHandler : IRequestHandler<CreatePackageCommand, Guid>
{
    private readonly RedFastDbContext _context;

    public CreatePackageHandler(RedFastDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreatePackageCommand request, CancellationToken cancellationToken)
    {
        var sender = await _context.Senders.FirstOrDefaultAsync(s => s.UserId == request.UserId, cancellationToken);
        if (sender ==  null)
            throw new InvalidOperationException("Remetente não encontrado.");

        var package = new Package
        {
            Id = Guid.NewGuid(),
            TrackingCode = GenerateTrackingCode(),
            SenderId = sender.Id,
            OriginAddress = request.OriginAddress,
            DestinationAddress = request.DestinationAddress,
            Weight = request.Weight,
            CreatedAt = DateTime.UtcNow
        };

        _context.Packages.Add(package);
        await _context.SaveChangesAsync(cancellationToken);

        return package.Id;
    }

    private static string GenerateTrackingCode()
    {
        var random = new Random();
        return $"BR{random.Next(100000000, 999999999)}";
    }
}
