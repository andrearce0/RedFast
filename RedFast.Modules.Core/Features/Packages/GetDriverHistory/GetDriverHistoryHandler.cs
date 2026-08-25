using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Persistence;

namespace RedFast.Modules.Core.Features.Packages.GetDriverHistory;

public class GetDriverHistoryHandler : IRequestHandler<GetDriverHistoryQuery, List<DriverHistoryViewModel>>
{
    private readonly RedFastDbContext _context;

    public GetDriverHistoryHandler(RedFastDbContext context)
    {
        _context = context;
    }

    public async Task<List<DriverHistoryViewModel>> Handle(GetDriverHistoryQuery query, CancellationToken cancellationToken)
    {
        var driver = await _context.Drivers
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.UserId == query.UserId, cancellationToken);

        if (driver == null)
            throw new InvalidOperationException("Motorista não encontrado");

        var packages = await _context.Packages
            .AsNoTracking()
            .Where(p => p.DriverId == driver.Id && (p.CurrentStatus == Entities.Enums.PackageStatus.DeliveryFailed || p.CurrentStatus == Entities.Enums.PackageStatus.Delivered))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new DriverHistoryViewModel
            (
                p.Id,
                p.OriginAddress,
                p.DestinationAddress,
                p.CurrentStatus.ToString(),
                p.Weight
            )).ToListAsync(cancellationToken);

        return packages;
    }
}
