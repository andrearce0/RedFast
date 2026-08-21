using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Entities.Enums;
using RedFast.Modules.Core.Persistence;

namespace RedFast.Modules.Core.Features.Packages.GetDriverActivePackages;

public class GetDriverActivePackagesHandler : IRequestHandler<GetDriverActivePackagesQuery, List<DriverActivePackageViewModel>>
{
    private readonly RedFastDbContext _context;

    public GetDriverActivePackagesHandler(RedFastDbContext context)
    {
        _context = context;
    }

    public async Task<List<DriverActivePackageViewModel>> Handle(GetDriverActivePackagesQuery query, CancellationToken cancellationToken)
    {
        var driver = await _context.Drivers
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.UserId == query.UserId, cancellationToken);

        if (driver == null)
            throw new InvalidOperationException("Motorista não encontrado");

        var packages = await _context.Packages
            .AsNoTracking()
            .Where(p => p.DriverId == driver.Id && (p.CurrentStatus == PackageStatus.Created ||p.CurrentStatus == PackageStatus.AwaitingPickup))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new DriverActivePackageViewModel
            (
                p.Id,
                p.TrackingCode,
                p.OriginAddress,
                p.DestinationAddress,
                p.Weight,
                p.CurrentStatus.ToString(),
                p.Sender!.CompanyName
            )).ToListAsync(cancellationToken);

        return packages;
    }
}
