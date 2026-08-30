using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Persistence;

namespace RedFast.Modules.Core.Features.Packages.GetPackageTracking;

public class GetPackageTrackingHandler : IRequestHandler<GetPackageTrackingQuery, PackageTrackingViewModel>
{
    private readonly RedFastDbContext _context;

    public GetPackageTrackingHandler(RedFastDbContext context)
    {
        _context = context;
    }

    public async Task<PackageTrackingViewModel> Handle(GetPackageTrackingQuery query, CancellationToken cancellationToken)
    {
        var packageTracking = await _context.Packages
            .AsNoTracking()
            .Where(p => p.TrackingCode == query.TrackingCode)
            .Select(p => new PackageTrackingViewModel
            (
                p.TrackingCode,
                p.CurrentStatus.ToString(),
                p.OriginAddress,
                p.DestinationAddress,
                p.Events
                    .OrderBy(e => e.Timestamp)
                    .Select(e => new TrackingEventViewModel
                    (
                        e.Status.ToString(),
                        e.Description,
                        e.Location,
                        e.Timestamp
                    )).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return packageTracking!;
    }
}
