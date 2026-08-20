using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Infrastructure.Messaging;
using RedFast.Modules.Core.Persistence;

namespace RedFast.Modules.Core.Features.Packages.AssignDriver;

public class AssignDriverHandler : IRequestHandler<AssignDriverCommand, Unit>
{
    private readonly RedFastDbContext _context;
    private readonly IMessageBus _messageBus;

    public AssignDriverHandler(RedFastDbContext context,
        IMessageBus messageBus)
    {
        _context = context;
        _messageBus = messageBus;
    }

    public async Task<Unit> Handle(AssignDriverCommand command, CancellationToken cancellationToken)
    {
        var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == command.UserId, cancellationToken);
        if(driver == null)
            throw new InvalidOperationException("Motorista informado não existe.");

        var package = await _context.Packages.FirstOrDefaultAsync(p => p.Id == command.PackageId, cancellationToken);
        if (package == null)
            throw new InvalidOperationException("Pacote informado não existe.");

        var oldStatus = package.CurrentStatus.ToString();

        package.AssignDriver(driver.Id);

        await _context.SaveChangesAsync(cancellationToken);

        var eventMessage = new PackageStatusChangedEvent(
                package.Id,
                oldStatus,
                package.CurrentStatus.ToString(),
                DateTimeOffset.UtcNow
            );

        await _messageBus.PublishAsync(eventMessage, "package.status.changed");

        return Unit.Value;
    }
}
