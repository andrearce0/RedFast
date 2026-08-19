using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Persistence;

namespace RedFast.Modules.Core.Features.Packages.GetAvailablePackages;

public class GetAvailablePackagesHandler : IRequestHandler<GetAvailablePackagesQuery, List<AvailablePackageViewModel>>
{
    private readonly RedFastDbContext _context;

    public GetAvailablePackagesHandler(RedFastDbContext context)
    {
        _context = context;
    }

    public async Task<List<AvailablePackageViewModel>> Handle(GetAvailablePackagesQuery request, CancellationToken cancellationToken)
    {
        var packages = await _context.Packages
            .AsNoTracking()
            .Where(p => p.CurrentStatus == Entities.Enums.PackageStatus.Created && p.Driver == null)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new AvailablePackageViewModel
            (
                p.Id,
                p.TrackingCode,
                p.OriginAddress,
                p.DestinationAddress,
                p.Weight,
                p.CreatedAt
            )).ToListAsync(cancellationToken);

        return packages;
    }
}
