using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Persistence;

namespace RedFast.Modules.Core.Features.Packages.GetSenderPackages;

public class GetSenderPackagesHandler : IRequestHandler<GetSenderPackagesQuery, List<PackageViewModel>>
{
    private readonly RedFastDbContext _context;

    public GetSenderPackagesHandler(RedFastDbContext context)
    {
        _context = context;
    }

    public async Task<List<PackageViewModel>> Handle(GetSenderPackagesQuery request, CancellationToken cancellationToken)
    {
        var sender = await _context.Senders
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == request.UserId);

        if(sender == null)
            return new List<PackageViewModel>();

        var packages = await _context.Packages
            .AsNoTracking()
            .Where(p => p.SenderId == sender.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PackageViewModel(
                p.Id,
                p.TrackingCode,
                p.OriginAddress,
                p.DestinationAddress,
                p.Weight,
                p.CurrentStatus.ToString(),
                p.CreatedAt
             ))
            .ToListAsync(cancellationToken);

        return packages;
    }
}
