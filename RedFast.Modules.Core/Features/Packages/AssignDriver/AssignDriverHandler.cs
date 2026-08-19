using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Persistence;

namespace RedFast.Modules.Core.Features.Packages.AssignDriver;

public class AssignDriverHandler : IRequestHandler<AssignDriverCommand, Unit>
{
    private readonly RedFastDbContext _context;

    public AssignDriverHandler(RedFastDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(AssignDriverCommand command, CancellationToken cancellationToken)
    {
        var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == command.UserId, cancellationToken);
        if(driver == null)
            throw new InvalidOperationException("Motorista informado não existe.");

        var package = await _context.Packages.FirstOrDefaultAsync(p => p.Id == command.PackageId, cancellationToken);
        if (package == null)
            throw new InvalidOperationException("Pacote informado não existe.");

        package.AssignDriver(driver.Id);

        _context.Packages.Update(package);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
